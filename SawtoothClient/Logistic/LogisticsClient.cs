using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using SawtoothClient.Objects;
using SharedObjects.Commands;
using SharedObjects.Enums;

namespace SawtoothClient.Logistic
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

        public BatchStatusResponse NewEntity(LogisticEnums.EntityType entityType, string creatorId)
        {
            var newGuid = Guid.NewGuid();
            var info = new Info()
            {
                EntityType = entityType,
                Id = creatorId
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                PublicKey = _publicKey,
                TransactionId = newGuid,
                Info = info,
                TimeStamp = DateTime.Now
            };

            var jsonCommand = JsonConvert.SerializeObject(command);
            var response = _sawtoothClient.PostBatch(jsonCommand);
            if(response.StatusCode != HttpStatusCode.Accepted)
                throw new HttpRequestException($"Message was not accepted! StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");

            var batchResponse = new BatchStatusResponse()
            {
                TransactionId = newGuid,
                //BatchId = response.Content
            };

            var batchStatus = _sawtoothClient.GetBatchStatuses("", 5);
            if (batchStatus.StatusCode is HttpStatusCode.OK)
            {



            }
            else
            {

            }
            return batchResponse;
        }

        public HttpResponseMessage AddEvent(Guid transactionId, LogisticEnums.EventType eventType, string jsonString)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }

        public HttpResponseMessage MakeFinal(Guid transactionId)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }

        public HttpResponseMessage NewInvite(Guid transactionId, byte[] invitedPublicKey)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }

        public HttpResponseMessage CancelInvite(Guid transactionId, byte[] invitedPublicKey)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }

        public HttpResponseMessage RejectInvite(Guid transactionId, byte[] invitedPublicKey)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }

        public HttpResponseMessage AcceptInvite(Guid transactionId, byte[] invitedPublicKey)
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
            return _sawtoothClient.PostBatch(jsonCommand);
        }
    }
}
