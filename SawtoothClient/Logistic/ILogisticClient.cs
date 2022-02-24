using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SawtoothClient.Objects;
using SharedObjects.Enums;

namespace SawtoothClient.Logistic
{
    public interface ILogisticClient
    {
        TransactionStatus NewEntity(LogisticEnums.EntityType entityType);
        TransactionStatus AddEvent(Guid transactionId, LogisticEnums.EventType eventType, string jsonString);
        TransactionStatus MakeFinal(Guid transactionId);
        TransactionStatus NewInvite(Guid transactionId, byte[] invitedPublicKey);
        TransactionStatus CancelInvite(Guid transactionId, byte[] invitedPublicKey);
        TransactionStatus RejectInvite(Guid transactionId, byte[] invitedPublicKey);
        TransactionStatus AcceptInvite(Guid transactionId, string jsonString);
    }
}
