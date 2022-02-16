using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SawtoothClient.Logistic;
using SharedObjects.Enums;
using SharedObjects.RideShare;

namespace SawtoothClient.RideSharing
{
    public class RideShareClient : LogisticsClient
    {
        private ILogisticClient _logisticClient;

        public RideShareClient(ILogisticClient logisticClient)
        {
            _logisticClient = logisticClient;
        }

        public int StartRide(string companyId, string driverId, string location)
        {
            var entityGuid = _logisticClient.NewEntity(LogisticEnums.EntityType.RideShare, companyId);
            
            var rideShareObj = new RideShareStruct
            {
                 DriverId = driverId,
                 Location = location
            };
            var jsonCommand = JsonConvert.SerializeObject(rideShareObj);

            _logisticClient.AddEvent(entityGuid, LogisticEnums.EventType.StartRide, jsonCommand);

            //Batch id(s)
            return 0;
        }

        public int AddPassenger(string passengerId, string location)
        {
            var rideShareObj = new RideShareStruct
            {
                EventType = LogisticEnums.EventType.AddPassenger,
                Location = location,
                PassengerId = passengerId
            };

            _logisticClient.AddEvent()

            //Batch id(s)
            return 0;
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
