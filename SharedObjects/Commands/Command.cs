using System;
using SharedObjects.Enums;

namespace SharedObjects.Commands
{
    public class Command
    {
        public LogisticEnums.Commands CommandType { get; set; }
        public Guid TransactionId { get; set; }
        public string CompanyId { get; set; }
        public Info Info { get; set; }
        public byte[] PublicKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
