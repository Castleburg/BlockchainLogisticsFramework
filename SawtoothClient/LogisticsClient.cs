using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SharedObjects.Commands;
using SharedObjects.Enums;

namespace SawtoothClient
{
    public class LogisticsClient
    {
        private readonly SawtoothClient _sawtoothClient;
        private readonly byte[] _publicKey;

        public LogisticsClient(byte[] publicKey, SawtoothClient client)
        {
            _publicKey = publicKey;
            _sawtoothClient = client;
        }

        public void NewEntity(LogisticEnums.EntityType entityType, string creatorId)
        {
            var info = new Info()
            {
                EntityType = entityType,
                Id = creatorId
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                PublicKey = _publicKey,
                TransactionId = Guid.NewGuid(),
                Info = info,
                TimeStamp = DateTime.Now
            };

            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void AddEvent(Guid transactionId, LogisticEnums.EventType eventType, string jsonString)
        {
            var info = new Info()
            {
                EventType = eventType,
                JsonContainer = jsonString
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.AddEvent,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void MakeFinal(Guid transactionId)
        {
            var info = new Info();
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.MakeFinal,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void NewInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info()
            {
                InvitePublicKey = invitedPublicKey
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewInvite,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void CancelInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info()
            {
                InvitePublicKey = invitedPublicKey
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.CancelInvite,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void RejectInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info();
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.RejectInvite,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }

        public void AcceptInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info()
            {
                InvitePublicKey = invitedPublicKey
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.AcceptInvite,
                PublicKey = _publicKey,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            var jsonCommand = JsonConvert.SerializeObject(command);
            _sawtoothClient.PostBatch(jsonCommand);
        }
    }
}
