using System.Collections.Generic;

namespace SharedObjects
{
    public class LogisticsEntity
    {
        public Operator Owner { get; set; }
        public List<Operator> Operator { get; set; }
        public List<Entity> Entities { get; set; }
    }
}
