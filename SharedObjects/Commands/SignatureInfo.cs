using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;

namespace SharedObjects.Commands
{
    public class SignatureInfo
    {
        public LogisticEnums.Commands CommandType { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
