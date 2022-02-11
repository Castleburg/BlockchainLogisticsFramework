using SharedObjects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.RideShare
{
    public class RideShare
    {
        public Guid DriverId { get; set; }
        public string Location { get; set; }
        public LogisticEnums.EventType EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public List<String> PassengerId { get; set; }
    }
}
