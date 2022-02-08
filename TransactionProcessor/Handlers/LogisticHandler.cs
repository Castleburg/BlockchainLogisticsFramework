using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeterO.Cbor;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using Sawtooth.Sdk.Client;
using Google.Protobuf;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using SharedObjects;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor.Process;
using TransactionProcessor.Process.Interfaces;

namespace TransactionProcessor.Handlers
{
    internal class LogisticHandler : ITransactionHandler
    {
        public string FamilyName { get; }
        public string Version { get; }
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly ILogisticProcess _logisticProcess;

        public LogisticHandler(string familyName, string version, ILogisticProcess logisticProcess)
        {
            FamilyName = familyName;
            Version = version;
            _logisticProcess = logisticProcess;
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());
            var command = obj["command"].ToObject<Command>();

            //TODO Check Digital Signature

            await HandleRequest(command, context);
        }

        private async Task HandleRequest(Command command, TransactionContext context)
        {
            var entity = command.Type switch
            {
                LogisticEnums.Commands.NewEntity    => _logisticProcess.NewEntity(command, context),
                LogisticEnums.Commands.AddEvent     => _logisticProcess.AddEvent(command, context),
                LogisticEnums.Commands.MakeFinal    => _logisticProcess.MakeFinal(command, context),

                LogisticEnums.Commands.NewInvite    => _logisticProcess.NewInvite(command, context),
                LogisticEnums.Commands.CancelInvite => _logisticProcess.CancelInvite(command, context),
                LogisticEnums.Commands.RejectInvite => _logisticProcess.RejectInvite(command, context),
                LogisticEnums.Commands.AcceptInvite => _logisticProcess.AcceptInvite(command, context),

                _ => throw new InvalidTransactionException($"Unknown ActionType {command.Type}")
            };
            await SaveState(command.TransactionId, entity, context);
        }

        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private async Task SaveState(Guid transactionId, Entity value, TransactionContext context)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            var t = await context.SetStateAsync(new Dictionary<string, ByteString>
            {
                { GetAddress(transactionId), ByteString.CopyFrom(jsonValue, Encoding.UTF8) }
            });
        }

    }
}
