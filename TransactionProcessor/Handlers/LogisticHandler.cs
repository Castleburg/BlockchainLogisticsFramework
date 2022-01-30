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
using SharedObjects.Logistic;

namespace TransactionProcessor.Handlers
{
    internal class LogisticHandler : ITransactionHandler
    {
        public string FamilyName => "Transport1";
        public string Version => "1.0";
        public string[] Namespaces => new[] { FamilyName.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString() };
        private string Prefix => FamilyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);
        
        public LogisticHandler()
        {
        }

        public async Task ApplyAsync(TpProcessRequest request, TransactionContext context)
        {
            var obj = CBORObject.DecodeFromBytes(request.Payload.ToByteArray());

            var commandContainer = obj["commandContainer"].ToObject<CommandContainer>();

            var state = await context.GetStateAsync(new[] { GetAddress(commandContainer.TransactionId) });

            if (NotInitNorInitialized(commandContainer.ActionType, state))
                throw new InvalidTransactionException("No address related to that TransactionId. Use the \"CreateNewLogisticEntity\" ActionType to create a new one.");
            if(AlreadyInitialized(commandContainer.ActionType, state))
                throw new InvalidTransactionException("Address already in use.");

            var value = commandContainer.ActionType switch
            {
                //"CreateNewContainer" => CreateNewContainer(commandContainer),
                //"AddOperator" => AddOperator(commandContainer, UnpackByteString(state.First().Value)),
                "AddEntity" => AddEntity(commandContainer, UnpackByteString(state.First().Value)),
                //"Update" => UpdateItem(UnpackByteString(state.First().Value), itemId, json),
                _ => throw new InvalidTransactionException($"Unknown ActionType {commandContainer.ActionType}")
            };
            await SaveState(commandContainer.TransactionId, value, context);
        }

        T[] Arrayify<T>(T obj) => new[] { obj };

        private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;
        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();
        private static bool NotInitNorInitialized(string action, Dictionary<string, ByteString> state) => !action.Equals("CreateNewLogisticEntity") && !state.Any();
        private static bool AlreadyInitialized(string action, Dictionary<string, ByteString> state) => action.Equals("CreateNewLogisticEntity") && state.Any();
        private static bool NotJson(string json)
        {
            try
            {
                JsonConvert.DeserializeObject(json);
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        }
        private static Container UnpackByteString(ByteString byteStringJson)
        {
            return JsonConvert.DeserializeObject<Container>(byteStringJson.ToStringUtf8());
        }

        private void HandleRequest(Command command, string json, TransactionContext context)
        {
            switch (command.Type)
            {
                case "CreateContainer":
                    CreateContainer(command,
                        JsonConvert.DeserializeObject<Operator>(json),
                        context);
                    break;
                case "AddOperator":
                    AddOperator(command,
                        JsonConvert.DeserializeObject<Operator>(json)
                        , context);
                    break;
                case "AddEntity":
                    AddEntity(command, JsonConvert.DeserializeObject<Operator>(json), context);
                    break;

                default:
                    // code block
                    break;
            }
        }

        private async void CreateContainer(Command command, Operator op, TransactionContext context)
        {
            var now = DateTime.Now;
            var container = new Container()
            {
                CreatedBy = op.PublicKey,
                Entities = new List<Entity>(),
                Operators = new List<Operator>() { op },
                CreatedDate = now,
                LastModified = now
            };
            await SaveState(command.TransactionId, container, context);
        }

        private async void AddOperator(Command command, Operator op, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            var container = UnpackByteString(state.First().Value);
            if (!container.CreatedBy.Equals(command.PublicKey))
                throw new InvalidTransactionException($"Only the owner can add new operators");
            container.Operators.Add(op);
            container.LastModified = DateTime.Now;

            await SaveState(command.TransactionId, container, context);
        }

        private async void AddEntity(Command command, CommandContainer commandContainer, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            var container = UnpackByteString(state.First().Value);
            var entity = new Entity()
            {
                CreatedByOperatorName = commandContainer.OperatorName,
                JsonString = commandContainer.Command.JsonEntity,
                TimeStamp = commandContainer.Command.TimeStamp
            };
            container.Entities.Add(entity);
            await SaveState(command.TransactionId, container, context);
        }

        private async Task SaveState(Guid transactionId, Container value, TransactionContext context)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            var t = await context.SetStateAsync(new Dictionary<string, ByteString>
            {
                { GetAddress(transactionId), ByteString.CopyFrom(jsonValue, Encoding.UTF8) }
            });
        }

    }
}
