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

namespace TransactionProcessor.Handlers
{
    internal class LogisticHandler : ITransactionHandler
    {
        public string FamilyName => "Transport1";
        public string Version => "1.0";
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly string _entityNotCreatedMessage =
            $"No address related to that TransactionId. Use the \"{LogisticEnums.Commands.NewEntity}\" ActionType to create a new one.";
        private readonly string _commandNullMessage = $"Command was null.";

        public LogisticHandler()
        {
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());
            var command = obj["command"].ToObject<Command>();

            HandleRequest(command, context);
        }

        //T[] Arrayify<T>(T obj) => new[] { obj };
        private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;
        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();
        private static bool AlreadyInitialized(LogisticEnums.Commands type, Dictionary<string, ByteString> state) => (type == LogisticEnums.Commands.NewEntity) && state.Any();
        private static Entity UnpackByteString(ByteString byteStringJson)
        {
            return JsonConvert.DeserializeObject<Entity>(byteStringJson.ToStringUtf8());
        }

        private void HandleRequest(Command command, TransactionContext context)
        {
            switch (command.Type)
            {
                case LogisticEnums.Commands.NewEntity:
                    NewEntity(command, context);
                    break;
                case LogisticEnums.Commands.AddEvent:
                    break;

                case LogisticEnums.Commands.Invite:
                    break;
                case LogisticEnums.Commands.CancelInvite:
                    break;
                case LogisticEnums.Commands.RejectInvite:
                    break;

                case LogisticEnums.Commands.Sign:
                    break;

                default:
                    throw new InvalidTransactionException($"Unknown ActionType {command.Type}");
            }
        }

        private async void NewEntity(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (AlreadyInitialized(command.Type, state))
                throw new InvalidTransactionException("Address already in use.");

            var newEntity = JsonConvert.DeserializeObject<NewEntity>(command.JsonContainer);
            if(newEntity is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var now = DateTime.Now;
            var entity = new Entity()
            {
                Type = newEntity.Type,
                CreatorId = command.PublicKey,
                TransactionId = command.TransactionId,
                Events = new List<SharedObjects.Logistic.Event>(),
                SignOff = new SignOff()
                {
                    Invites = new List<Invite>(),
                    Signatories = new List<Operator>()
                },
                Completed = false,
                CreatedDate = now,
                LastModified = now
            };
            await SaveState(command.TransactionId, entity, context);
        }

        //private async void AddOperator(Command command, Operator op, TransactionContext context)
        //{
        //    var state = GetState(command.TransactionId, context);
        //    var container = UnpackByteString(state.First().Value);
        //    if (!container.CreatedBy.Equals(command.PublicKey))
        //        throw new InvalidTransactionException($"Only the owner can add new operators");
        //    container.Operators.Add(op);
        //    container.LastModified = DateTime.Now;

        //    await SaveState(command.TransactionId, container, context);
        //}

        

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
