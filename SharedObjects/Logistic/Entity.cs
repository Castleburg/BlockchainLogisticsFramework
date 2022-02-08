using System;
using System.Collections.Generic;
using SharedObjects.Enums;

namespace SharedObjects.Logistic
{
    public class Entity
    {
        public LogisticEnums.EntityType Type { get; set; }
        public Guid TransactionId { get; set; }
        public string CreatorId { get; set; } //PublicKey
        public List<Event> Events { get; set; }
        public SignOff SignOff { get; set; }
        public bool Final { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }
}
