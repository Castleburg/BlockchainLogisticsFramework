using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects
{
    public class Operator
    {
        public string PublicKey { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public List<User> Users { get; set; }
        public HashSet<string> PermittedEntities { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
