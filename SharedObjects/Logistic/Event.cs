using System;
using SharedObjects.Enums;

namespace SharedObjects.Logistic
{
    public class Event
    {
        public Event(LogisticEnums.EventType type, string jsonContainer, DateTime timeStamp)
        {
            Type = type;
            JsonContainer = jsonContainer;
            TimeStamp = timeStamp;
        }
        public LogisticEnums.EventType Type { get; set; }
        public string JsonContainer { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
