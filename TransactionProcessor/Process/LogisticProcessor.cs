using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Google.Protobuf;
using Newtonsoft.Json;
using Sawtooth.Sdk;
using Sawtooth.Sdk.Processor;
using SharedObjects;
using SharedObjects.Commands;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using TransactionProcessor.Process.BusinessProcesses;

namespace TransactionProcessor.Process
{
    internal class LogisticProcessor : ILogisticProcessor
    {
        private readonly string _familyName;
        private readonly string _sawtoothApiAddress;
        private string Prefix => _familyName.ToByteArray().ToSha512().ToHexString().Substring(0, 6);

        private readonly IBusinessProcess _businessProcess;
        private readonly HttpClient _httpClient;

        private readonly string _entityNotCreatedMessage = $"No address related to that TransactionId. Use the \"{LogisticEnums.Commands.NewEntity}\" ActionType to create a new one.";
        private readonly string _entityFinalMessage = "Entity is final and can no longer be changed.";
        private readonly string _entityNotFinalMessage = "Entity has not yet been made final.";
        private readonly string _inviteAlreadyExists = "Cannot invite the same signatory more than once.";
        private readonly string _inviteDoesNotExists = "No invite with that public key exists.";
        private readonly string _unauthorized = "Unauthorized to make changes to the entity.";
        private readonly string _inviteNotPending = "Invite is not pending.";


        public LogisticProcessor(string familyName, string sawtoothApiAddress, IBusinessProcess businessProcess)
        {
            _familyName = familyName;
            _businessProcess = businessProcess;
            _sawtoothApiAddress = sawtoothApiAddress;
            _httpClient = new HttpClient();
        }

        public Entity NewEntity(Command command, TransactionContext context)
        {
            var state = GetEntityFromState(command.TransactionId);
            if (state is null)
                throw new InvalidTransactionException("Address already in use.");

            if (command.Info.EntityType == LogisticEnums.EntityType.Undefined)
                throw new InvalidTransactionException("EntityType is undefined.");

            var entity = new Entity()
            {
                Type = command.Info.EntityType,
                PublicKey = command.PublicKey,
                CreatorId = command.CompanyId,
                TransactionId = command.TransactionId,
                Events = new List<CustomEvent>(),
                SignOff = new SignOff()
                {
                    Invites = new List<Invite>(),
                    Signatories = new List<Signatory>()
                },
                Final = false,
                CreatedDate = command.TimeStamp,
                LastModified = command.TimeStamp
            };
            return entity;
        }

        public Entity AddEvent(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);

            if (command.Info.EventType == LogisticEnums.EventType.Undefined)
                throw new InvalidTransactionException("EventType is undefined.");

            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            var eventsCopy = EventsDeepCopy(entity.Events);
            var cEvent = new CustomEvent(command.Info.EventType, command.Info.JsonContainer, command.TimeStamp);

            var newEvent = _businessProcess.AddEvent(cEvent, eventsCopy);

            entity.LastModified = command.TimeStamp;
            entity.Events.Add(newEvent);
            return entity;
        }

        public Entity MakeFinal(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);

            if (NotEntityCreator(command.PublicKey, entity))
                throw new InvalidTransactionException(_unauthorized);

            if (entity.Final)
                throw new InvalidTransactionException(_entityFinalMessage);

            var eventsCopy = EventsDeepCopy(entity.Events);
            if(!_businessProcess.MakeFinal(eventsCopy))
                throw new InvalidTransactionException("Entity not eligible to be made final.");

            entity.LastModified = command.TimeStamp;
            entity.Final = true;
            return entity;
        }

        public Entity NewInvite(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);

            if (command.Info.InvitePublicKey is null)
                throw new InvalidTransactionException("Missing invited public key.");
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

            entity.LastModified = command.TimeStamp;
            entity.SignOff.Invites.Add(invite);
            return entity;
        }

        public Entity CancelInvite(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);

            if (command.Info.InvitePublicKey is null)
                throw new InvalidTransactionException("Missing invited public key.");
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

            entity.LastModified = command.TimeStamp;
            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.Info.InvitePublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Cancelled;
            });

            return entity;
        }

        public Entity RejectInvite(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);

            if (!entity.Final)
                throw new InvalidTransactionException(_entityNotFinalMessage);

            var matchingInvites = entity.SignOff.Invites.FindAll(x => PublicKeyComparison(x.PublicKey, command.PublicKey));
            if (!matchingInvites.Any())
                throw new InvalidTransactionException(_inviteDoesNotExists);

            var invite = matchingInvites.First();
            if (invite.InviteStatus != LogisticEnums.InviteStatus.Pending)
                throw new InvalidTransactionException(_inviteNotPending);

            entity.LastModified = command.TimeStamp;
            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.PublicKey)){}
                    x.InviteStatus = LogisticEnums.InviteStatus.Rejected;
            });

            return entity;
        }

        public Entity AcceptInvite(Command command, TransactionContext context)
        {
            var entity = GetEntityFromState(command.TransactionId);
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

            entity.LastModified = command.TimeStamp;
            entity.SignOff.Invites.ForEach(x =>
            {
                if (PublicKeyComparison(x.PublicKey, command.PublicKey))
                    x.InviteStatus = LogisticEnums.InviteStatus.Signed;
            });
            
            var newSignatory = new Signatory(command.CompanyId, command.PublicKey, jsonInviteResponse, DateTime.Now);

            entity.SignOff.Signatories.Add(newSignatory);
            return entity;
        }

        //public Entity GetEntityFromState(Command command, TransactionContext context)
        //{
        //    var state = GetState(command.TransactionId);
        //    if (!state.Any())
        //        throw new InvalidTransactionException(_entityNotCreatedMessage);
        //    return UnpackByteString(state.First().Value);
        //}

        //private Dictionary<string, ByteString> GetState(Guid transactionId, TransactionContext context) => context.GetStateAsync(new[] { GetAddress(transactionId) }).Result;

        public Entity GetEntityFromState(Guid transactionId)
        {
            var stateAddress = GetAddress(transactionId);
            var response = GetStateHttp(stateAddress);
            if(!response.IsSuccessStatusCode)
                throw new InvalidTransactionException("Unable to fetch transaction state");

            var content = response.Content.ReadAsStringAsync().Result;
            var stateResponse = JsonConvert.DeserializeObject<StateResponse>(content);

            var data = Convert.FromBase64String(stateResponse.Data);
            var decodedString = Encoding.UTF8.GetString(data);

            var state = JsonConvert.DeserializeObject<Entity>(decodedString);
            return state;
        }

        private HttpResponseMessage GetStateHttp(string stateAddress)
        {
            var builder = new StringBuilder();
            builder.Append(_sawtoothApiAddress);
            builder.Append("/state/");
            builder.Append(stateAddress);
            var request = builder.ToString();
            return _httpClient.GetAsync(request).Result;
        }

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
