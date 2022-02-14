using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using Google.Protobuf;
using Newtonsoft.Json;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor.Process;
using TransactionProcessor.Tools.Interfaces;

namespace TransactionProcessor.Handlers
{
    internal class LogisticHandler : ITransactionHandler
    {
        public string FamilyName { get; }
        public string Version { get; }
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly ILogisticProcessor _logisticProcess;
        private readonly ICryptographicService _cryptographicService;

        public LogisticHandler(string familyName, string version, ILogisticProcessor logisticProcess, ICryptographicService cryptographicService)
        {
            FamilyName = familyName;
            Version = version;
            _logisticProcess = logisticProcess;
            _cryptographicService = cryptographicService;
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            //var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());
            //var token = obj["command"].ToObject<CommandToken>();

            var bytes = request.Payload.ToByteArray();
            var stringPayload = Encoding.UTF8.GetString(bytes);
            Token token = null;
            try
            {
                token = JsonConvert.DeserializeObject<Token>(stringPayload);
            }
            catch
            {
                throw new InvalidTransactionException($"Could not unpack Token.");
            }
            
            if(token is null)
                throw new InvalidTransactionException($"Token was null.");

            if (!_cryptographicService.VerifySignature(token))
                throw new InvalidTransactionException($"Digital Signature was invalid.");

            await HandleRequest(token.Command, context);
        }

        private async Task HandleRequest(Command command, TransactionContext context)
        {
            var entity = command.CommandType switch
            {
                LogisticEnums.Commands.NewEntity    => _logisticProcess.NewEntity(command, context),
                LogisticEnums.Commands.AddEvent     => _logisticProcess.AddEvent(command, context),
                LogisticEnums.Commands.MakeFinal    => _logisticProcess.MakeFinal(command, context),

                LogisticEnums.Commands.NewInvite    => _logisticProcess.NewInvite(command, context),
                LogisticEnums.Commands.CancelInvite => _logisticProcess.CancelInvite(command, context),
                LogisticEnums.Commands.RejectInvite => _logisticProcess.RejectInvite(command, context),
                LogisticEnums.Commands.AcceptInvite => _logisticProcess.AcceptInvite(command, context),

                _ => throw new InvalidTransactionException($"Unknown ActionType {command.CommandType}.")
            };
            //TODO: Check response
            var response = await SaveState(command.TransactionId, entity, context);
        }

        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private Task<string[]> SaveState(Guid transactionId, Entity value, TransactionContext context)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            return context.SetStateAsync(new Dictionary<string, ByteString>
            {
                { GetAddress(transactionId), ByteString.CopyFrom(jsonValue, Encoding.UTF8) }
            });
        }

    }
}
