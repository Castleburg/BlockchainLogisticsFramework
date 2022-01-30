using System;

namespace SharedObjects.Commands
{
    public class Command
    {
        public string Type { get; set; }
        public Guid TransactionId { get; set; }
        public string CommandJson { get; set; }
        public string PublicKey { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
