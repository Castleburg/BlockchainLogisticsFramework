using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects
{
    public class Entity
    {
        public Operator Owner { get; set; }
        public string JsonString { get; set; }
    }
}
