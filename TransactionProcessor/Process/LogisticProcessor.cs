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
using TransactionProcessor.Process.BusinessProcesses;

namespace TransactionProcessor.Process
{
    internal class LogisticProcessor : ILogisticProcessor
    {
        private readonly string _familyName;
        private string Prefix => _familyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly IBusinessProcess _businessProcess;

        private readonly string _entityNotCreatedMessage = $"No address related to that TransactionId. Use the \"{LogisticEnums.Commands.NewEntity}\" ActionType to create a new one.";
        private readonly string _entityFinalMessage = "Entity is final and can no longer be changed.";
        private readonly string _entityNotFinalMessage = "Entity has not yet been made final.";
        private readonly string _inviteAlreadyExists = "Cannot invite the same signatory more than once.";
        private readonly string _inviteDoesNotExists = "No invite with that public key exists.";
        private readonly string _unauthorized = "Unauthorized to make changes to the entity.";
        private readonly string _inviteNotPending = "Invite is not pending.";


        public LogisticProcessor(string familyName, IBusinessProcess businessProcess)
        {
            _familyName = familyName;
            _businessProcess = businessProcess;
        }

        public Entity NewEntity(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (state.Any())
                throw new InvalidTransactionException("Address already in use.");

            if (command.Info.EntityType == LogisticEnums.EntityType.Undefined)
                throw new InvalidTransactionException("EntityType is undefined.");

            var now = DateTime.Now;
            var entity = new Entity()
            {
                Type = command.Info.EntityType,
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

            if (command.Info.EventType == LogisticEnums.EventType.Undefined)
                throw new InvalidTransactionException("EventType is undefined.");



            var entity = UnpackByteString(state.First().Value);

            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            var eventsCopy = EventsDeepCopy(entity.Events);
            var newEvent = new CustomEvent(command.Info.EventType, command.Info.JsonContainer, command.TimeStamp);

            _businessProcess.AddEvent(newEvent, eventsCopy);

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

            var eventsCopy = EventsDeepCopy(entity.Events);
            if(!_businessProcess.MakeFinal(eventsCopy))
                throw new InvalidTransactionException("Entity not eligible to be made final.");

            entity.Final = true;
            return entity;
        }

        public Entity NewInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

            if (command.Info.InvitePublicKey is null)
                throw new InvalidTransactionException("Missing invited public key.");

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);

            if(InviteExists(entity, command.Info.InvitePublicKey))
                throw new InvalidTransactionException(_inviteAlreadyExists);

            var now = DateTime.Now;
            var invite = new Invite()
            {
                PublicKey = command.Info.InvitePublicKey,
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

            if (command.Info.InvitePublicKey is null)
                throw new InvalidTransactionException("Missing invited public key.");

            var entity = UnpackByteString(state.First().Value);
            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);

            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, command.Info.InvitePublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.Info.InvitePublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Cancelled;
            });

            return entity;
        }

        public Entity RejectInvite(Command command, TransactionContext context)
        {
            var state = GetState(command.TransactionId, context);
            if (!state.Any())
                throw new InvalidTransactionException(_entityNotCreatedMessage);

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

            var entity = UnpackByteString(state.First().Value);
            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);


            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, command.PublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            var signatoriesCopy = SignatoriesDeepCopy(entity.SignOff.Signatories);

            var jsonInviteResponse = _businessProcess.AcceptInvite(command.Info.JsonContainer, signatoriesCopy);

            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.PublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Signed;
            });

            var newSignatory = new Signatory(command.Info.Id, command.PublicKey, jsonInviteResponse, DateTime.Now);

            entity.SignOff.Signatories.Add(newSignatory);
            return entity;
        }

        private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;

        private string GetAddress(Guid transactionId) => Prefix + transactionId.ToByteArray().ToSha512().TakeLast(32).ToArray().ToHexString();

        private List<Signatory> SignatoriesDeepCopy(List<Signatory> signatories)
        {
            return signatories.ConvertAll(s =>
                new Signatory(s.Id, s.PublicKey, s.JsonContainer, s.SignedAt));
        }

        private List<CustomEvent> EventsDeepCopy(List<CustomEvent> events)
        {
            return events.ConvertAll(e =>
                new CustomEvent(e.Type, e.JsonContainer, e.TimeStamp));
        }

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
