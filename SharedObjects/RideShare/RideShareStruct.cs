using SharedObjects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.RideShare
{
    public class RideShareStruct
    {
        public string DriverId { get; set; }
        public string Location { get; set; }
        public List<string> PassengerIdList { get; set; }
        public string PassengerId { get; set; }
        public RideShareEnums.RideStatus Status { get; set; }
    }
}
