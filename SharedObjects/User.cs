using System;
using System.Collections.Generic;

namespace SharedObjects
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Active { get; set; }
        public HashSet<string> PermittedEntities { get; set; }
    }
}
