using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;
using SharedObjects.RideShare;

namespace SawtoothClient.RideSharing
{
    class RideShareClientCalls
    {
        public static RideShareStruct StartRide(string driverId,
                                          string location)
        {
            var rideShareObj = new RideShareStruct
            {
                 DriverId = driverId,
                 Location = location
            };

            return rideShareObj;
        }

        public static RideShareStruct AddPassenger(string passengerId, string location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.AddPassenger,
                Location = location,
                PassengerId = passengerId
            };
            return rideShareObj;
        }

        public static RideShareStruct RemovePassenger(string passengerId, string location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.RemovePassenger,
                PassengerId = passengerId,
                Location = location
            };

            return rideShareObj;
        }

        public static RideShareStruct CancelRide(string driverId, string location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.CancelRide,
                DriverId = driverId,
                Location = location
            };
            return rideShareObj;
        }

        public static RideShareStruct UpdateRide(string Location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.UpdateRide,
                Location = Location
            };
            return rideShareObj;
        }

        public static RideShareStruct StopRide(string driverId, string location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.StopRide,
                DriverId = driverId,
                Location = location
            };
            return rideShareObj;
        }

    }
}
