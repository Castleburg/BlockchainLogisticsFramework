using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;

namespace SawtoothClient.Objects
{
    public class TransactionStatus
    {
        public LogisticEnums.Commands Command;
        public SawtoothEnums.BatchStatus Status;
        public Guid TransactionId;
        public string BatchId;
    }
}
