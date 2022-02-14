using System;
using SharedObjects.Enums;

namespace SharedObjects.Logistic
{
    public class CustomEvent
    {
        public LogisticEnums.EventType Type { get; set; }
        public string JsonContainer { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
