using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SharedObjects.Enums;

namespace SawtoothClient.Logistic
{
    public interface ILogisticClient
    {
        Guid NewEntity(LogisticEnums.EntityType entityType, string creatorId);
        HttpResponseMessage AddEvent(Guid transactionId, LogisticEnums.EventType eventType, string jsonString);


    }
}
