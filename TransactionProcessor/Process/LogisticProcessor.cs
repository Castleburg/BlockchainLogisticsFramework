using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        private readonly string _entityNotFinalMessage = "Entity has not yet been made final.";
        private readonly string _inviteAlreadyExists = "Cannot invite the same signatory more than once.";
        private readonly string _inviteDoesNotExists = "No invite with that public key exists.";
        private readonly string _unauthorized = "Unauthorized to make changes to the entity.";
        private readonly string _inviteNotPending = "Invite is not pending.";


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
                Events = new List<SharedObjects.Logistic.Event>(),
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

            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            var newEvent = new SharedObjects.Logistic.Event()
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

            var entity = UnpackByteString(state.First().Value);
            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

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
                throw new InvalidTransactionException(_entityNotFinalMessage);

            if(InviteExists(entity, newInvite.InvitedPublicKey))
                throw new InvalidTransactionException(_inviteAlreadyExists);

            var now = DateTime.Now;
            var invite = new Invite()
            {
                PublicKey = newInvite.InvitedPublicKey,
                InviteStatus = LogisticEnums.InviteStatus.Pending,
                LastUpdated = now,
                CreatedDate = now
            };

            entity.SignOff.Invites.Add(invite);
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
            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);

            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, cancelInvite.InvitedPublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, cancelInvite.InvitedPublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Cancelled;
            });

            return entity;
        }

        public Entity RejectInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            //var rejectInvite = JsonConvert.DeserializeObject<RejectInvite>(command.JsonContainer);
            //if (rejectInvite is null)
            //    throw new InvalidTransactionException(_rejectInviteNoReason);

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);

            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, command.PublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.PublicKey)){}
                    x.InviteStatus = LogisticEnums.InviteStatus.Rejected;
            });

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
                throw new InvalidTransactionException(_entityNotFinalMessage);


            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, command.PublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            //Check with business logic
            //Create a signatory

            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.PublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Signed;
            });

            return entity;
        }

        private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;

        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private static Entity UnpackByteString(ByteString byteStringJson)
        {
            return JsonConvert.DeserializeObject<Entity>(byteStringJson.ToStringUtf8());
        }

        private bool PublicKeyComparison(byte[] key1, byte[] key2)
        {
            return key1.SequenceEqual(key2);
        }

        private bool InviteExists(Entity entity, byte[] invitedPublicKey)
        {
            return entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, invitedPublicKey))
                .Any();
        }

        private bool NotEntityCreator(byte[] publicKey, Entity entity)
        {
            return PublicKeyComparison(publicKey, entity.PublicKey);
        }
    }
}
