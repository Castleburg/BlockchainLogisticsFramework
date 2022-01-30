using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects
{
    public class CommandContainer
    {
        public string ActionType { get; set; }
        public Guid TransactionId { get; set; }
        public Command Command { get; set; }
        public string OperatorName { get; set; }
        public string SignedCommand { get; set; }
    }
}
