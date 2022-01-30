using System;
using System.Collections.Generic;

namespace SharedObjects.Logistic
{
    public class Container
    {
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public List<Operator> Operators { get; set; }
        public List<Entity> Entities { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }
}
