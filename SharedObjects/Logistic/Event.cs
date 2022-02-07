using System;

namespace SharedObjects.Logistic
{
    public class Event
    {
        public string Type { get; set; }
        public string JsonString { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
