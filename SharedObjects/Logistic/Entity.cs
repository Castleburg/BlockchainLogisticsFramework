using System;
using System.Collections.Generic;

namespace SharedObjects.Logistic
{
    public class Entity
    {
        public string Type { get; set; }
        public Operator Operator { get; set; }
        public List<Event> Events { get; set; }
        public SignOff SignOff { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }
}
