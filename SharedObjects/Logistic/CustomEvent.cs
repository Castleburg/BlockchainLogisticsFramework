using System;
using SharedObjects.Enums;

namespace SharedObjects.Logistic
{
    public class CustomEvent
    {
        public CustomEvent()
        {
        }

        public CustomEvent(LogisticEnums.EventType type, string jsonContainer, DateTime timeStamp)
        {
            Type = type;
            JsonContainer = jsonContainer;
            TimeStamp = timeStamp;
        }
        public LogisticEnums.EventType Type { get; set; }
        public string JsonContainer { get; set; }
        public DateTime TimeStamp { get; set; }

        //Make an unpack method
    }
}
