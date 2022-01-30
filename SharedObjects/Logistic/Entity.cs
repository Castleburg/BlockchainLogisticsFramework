using System;

namespace SharedObjects.Logistic
{
    public class Entity
    {
        public string CreatedByOperatorName { get; set; }
        public string Type { get; set; }
        public string JsonString { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Active { get; set; }
    }
}
