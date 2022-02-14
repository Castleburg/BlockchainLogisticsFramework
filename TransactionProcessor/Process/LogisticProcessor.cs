using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Newtonsoft.Json;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor.Process.Interfaces;

namespace TransactionProcessor.Process
{
    internal class LogisticProcessor : ILogisticProcessor
    {
        private readonly string _familyName;
        private string Prefix => _familyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly string _entityNotCreatedMessage = $"No address related to that TransactionId. Use the \"{LogisticEnums.Commands.NewEntity}\" ActionType to create a new one.";
        private readonly string _commandNullMessage = "Command was null.";
        private readonly string _entityFinalMessage = "Entity is final and can no longer be changed.";

        public LogisticProcessor(string familyName)
        {
            _familyName = familyName;
        }

        public Entity NewEntity(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (state.Any())
                throw new InvalidTransactionException("Address already in use.");

            var newEntity = JsonConvert.DeserializeObject<NewEntity>(command.JsonContainer);
            if (newEntity is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var now = DateTime.Now;
            var entity = new Entity()
            {
                Type = newEntity.Type,
                PublicKey = command.PublicKey,
                TransactionId = command.TransactionId,
                Events = new List<SharedObjects.Logistic.CustomEvent>(),
                SignOff = new SignOff()
                {
                    Invites = new List<Invite>(),
                    Signatories = new List<Signatory>()
                },
                Final = false,
                CreatedDate = now,
                LastModified = now
            };
            return entity;
        }

        public Entity AddEvent(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            var addEvent = JsonConvert.DeserializeObject<AddEvent>(command.JsonContainer);
            if (addEvent is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (entity.Final)
                throw new InvalidTransactionException("Entity is final and can no longer be changed.");

            var newEvent = new SharedObjects.Logistic.CustomEvent()
            {
                Type = addEvent.Type,
                JsonContainer = addEvent.JsonContainer,
                TimeStamp = DateTime.Now
            };

            //Check contents of JsonContainer based on Type



            entity.Events.Add(newEvent);
            return entity;
        }

        public Entity MakeFinal(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            //TODO
            var addEvent = JsonConvert.DeserializeObject<AddEvent>(command.JsonContainer);
            if (addEvent is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            //Check if Entity is valid to be made final

            return entity;
        }


        public Entity NewInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            var newInvite = JsonConvert.DeserializeObject<NewInvite>(command.JsonContainer);
            if (newInvite is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            //Check if invite has already been sent

            return entity;
        }

        public Entity CancelInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            var cancelInvite = JsonConvert.DeserializeObject<CancelInvite>(command.JsonContainer);
            if (cancelInvite is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            //Check if invite has already been sent

            return entity;
        }

        public Entity RejectInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            var rejectInvite = JsonConvert.DeserializeObject<RejectInvite>(command.JsonContainer);
            if (rejectInvite is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            //Check if invite has already been sent

            return entity;
        }

        public Entity AcceptInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            var acceptInvite = JsonConvert.DeserializeObject<AcceptInvite>(command.JsonContainer);
            if (acceptInvite is null)
                throw new InvalidTransactionException(_commandNullMessage);

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            //Check if invite has already been sent

            return entity;
        }

        private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;
        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();
        private static Entity UnpackByteString(ByteString byteStringJson)
        {
            return JsonConvert.DeserializeObject<Entity>(byteStringJson.ToStringUtf8());
        }
    }
}
