using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using SawtoothClient.Objects;
using SawtoothClient.Tools;
using SharedObjects.Commands;
using SharedObjects.Enums;

namespace SawtoothClient.Logistic
{
    public class LogisticsClient
    {
        private readonly string _companyId;
        private readonly byte[] _publicKey;
        private readonly SawtoothClient _sawtoothClient;
        private readonly RsaEncryptionService _encryptionService;

        public LogisticsClient(string companyId, byte[] publicKey, SawtoothClient client, RsaEncryptionService encryptionService)
        {
            _publicKey = publicKey;
            _companyId = companyId;
            _sawtoothClient = client;
            _encryptionService = encryptionService;
        }

        public TransactionStatus NewEntity(LogisticEnums.EntityType entityType)
        {
            var newGuid = Guid.NewGuid();
            var info = new Info()
            {
                EntityType = entityType
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewEntity,
                CompanyId = _companyId,
                PublicKey = _publicKey,
                TransactionId = newGuid,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        public TransactionStatus AddEvent(Guid transactionId, LogisticEnums.EventType eventType, string jsonString)
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
                CompanyId = _companyId,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        public TransactionStatus MakeFinal(Guid transactionId)
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
            return ExecuteBatch(command);
        }

        public TransactionStatus NewInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info()
            {
                InvitePublicKey = invitedPublicKey
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.NewInvite,
                PublicKey = _publicKey,
                CompanyId = _companyId,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        public TransactionStatus CancelInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            var info = new Info()
            {
                InvitePublicKey = invitedPublicKey
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.CancelInvite,
                PublicKey = _publicKey,
                CompanyId = _companyId,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        public TransactionStatus RejectInvite(Guid transactionId)
        {
            var info = new Info();
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.RejectInvite,
                PublicKey = _publicKey,
                CompanyId = _companyId,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        public TransactionStatus AcceptInvite(Guid transactionId, string jsonString)
        {
            var info = new Info()
            {
                JsonContainer = jsonString
            };
            var command = new Command()
            {
                CommandType = LogisticEnums.Commands.AcceptInvite,
                PublicKey = _publicKey,
                CompanyId = _companyId,
                TransactionId = transactionId,
                Info = info,
                TimeStamp = DateTime.Now
            };
            return ExecuteBatch(command);
        }

        private TransactionStatus ExecuteBatch(Command command)
        {
            var token = _encryptionService.AddSignature(command);
            var jsonToken = JsonConvert.SerializeObject(token);
            var response = _sawtoothClient.PostBatch(jsonToken);

            if (response.StatusCode != HttpStatusCode.Accepted)
                throw new HttpRequestException($"Message was not accepted! StatusCode: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");

            var content = response.Content.ReadAsStringAsync().Result;
            var link = JsonConvert.DeserializeObject<BatchResponse>(content);
            if (link is null)
                throw new HttpRequestException($"Could not unpack content! StatusCode: {response.StatusCode}, Content: {content}");

            var batchId = link.Link.Substring(link.Link.LastIndexOf('=') + 1);

            var transactionStatus = new TransactionStatus()
            {
                TransactionId = token.Command.TransactionId,
                BatchId = batchId,
                Command = LogisticEnums.Commands.NewEntity,
                Status = SawtoothEnums.BatchStatus.Unknown
            };

            var batchStatus = _sawtoothClient.GetBatchStatuses(batchId, 15);
            if (!(batchStatus.StatusCode is HttpStatusCode.OK)) return transactionStatus;

            var batchContent = batchStatus.Content.ReadAsStringAsync().Result;
            var batchStatusResponse = JsonConvert.DeserializeObject<BatchStatusResponse>(batchContent);
            if (batchStatusResponse is null)
                return transactionStatus;

            var status = batchStatusResponse.Data[0].GetStatus();
            transactionStatus.Status = status;
            return transactionStatus;
        }
    }
}
