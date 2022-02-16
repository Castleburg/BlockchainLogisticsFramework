using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using SawtoothClient.Objects;
using SawtoothClient.Tools;
using SharedObjects.Commands;
using SharedObjects.Enums;

namespace SawtoothClient.Logistic
{
    public class LogisticsClient
    {
        private readonly byte[] _publicKey;
        private readonly SawtoothClient _sawtoothClient;
        private readonly RsaEncryptionService _encryptionService;

        public LogisticsClient(byte[] publicKey, SawtoothClient client, RsaEncryptionService encryptionService)
        {
            _publicKey = publicKey;
            _sawtoothClient = client;
            _encryptionService = encryptionService;
        }

        public TransactionStatus NewEntity(LogisticEnums.EntityType entityType, string creatorId)
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

            var token = _encryptionService.SignCommand(command);

            var jsonToken = JsonConvert.SerializeObject(token);
            var response = _sawtoothClient.PostBatch(jsonToken);
            
            if (response.StatusCode != HttpStatusCode.Accepted)
                throw new HttpRequestException($"Message was not accepted! StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");

            var content = response.Content.ReadAsStringAsync().Result;
            var link = JsonConvert.DeserializeObject<BatchResponse>(content);
            if(link is null)
                throw new HttpRequestException($"Could not unpack content! StatusCode: {response.StatusCode}, Content: {content}");

            var batchId = link.Link.Substring(link.Link.LastIndexOf('=') + 1);

            var transactionStatus = new TransactionStatus()
            {
                TransactionId = newGuid,
                BatchId = batchId,
                Command = LogisticEnums.Commands.NewEntity,
                Status = SawtoothEnums.BatchStatus.Unknown
            };

            var batchStatus = _sawtoothClient.GetBatchStatuses(batchId, 10);
            if (!(batchStatus.StatusCode is HttpStatusCode.OK)) return transactionStatus;

            var batchContent = batchStatus.Content.ReadAsStringAsync().Result;
            var batchStatusResponse = JsonConvert.DeserializeObject<BatchStatusResponse>(batchContent);
            if (batchStatusResponse is null)
                return transactionStatus;

            var status = batchStatusResponse.Data[0].GetStatus();
            transactionStatus.Status = status;
            return transactionStatus;
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
