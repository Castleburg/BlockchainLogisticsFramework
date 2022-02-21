using SharedObjects.Enums;
using SharedObjects.Logistic;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.RideShare
{
    public class RideShareSignatoryReward
    {
        public string Location { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int GrantedTime { get; set; } //Minutes
    }
}
