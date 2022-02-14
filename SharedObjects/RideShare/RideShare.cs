﻿using SharedObjects.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.RideShare
{
    public class RideShare
    {
        public string DriverId { get; set; }
        public string Location { get; set; }
        public LogisticEnums.EventType EventType { get; set; }
        public List<string> PassengerIdList { get; set; }
        public string PassengerId { get; set; }
        public RideShareEnums.RideStatus Status { get; set; }
    }
}
