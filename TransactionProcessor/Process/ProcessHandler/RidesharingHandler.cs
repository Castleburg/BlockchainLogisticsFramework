using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using SharedObjects.RideShare;

namespace TransactionProcessor.Process.ProcessHandler
{
    internal class RidesharingHandler : IRidesharingHandler
    {
        public CustomEvent AddPassenger(string json, Entity entity)
        {
            JObject obj = JObject.Parse(json);

            var addPassengerRideShareEntity = new RideShare
            {
                DriverId = (Guid)   obj["DriverId"],
                PassengerId =  (List<string>) obj["DriverId"],
                Location = (string) obj["Location"],
                EventType = LogisticEnums.EventType.AddPassenger,
                Timestamp = DateTime.Now
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity)
            };
            return newEvent;
        }

        public CustomEvent CancelRide(string json, Entity entity)
        {
            JObject obj = JObject.Parse(json);

            var addPassengerRideShareEntity = new RideShare
            {
                DriverId = (Guid)   obj["DriverId"],
                Location = (string) obj["Location"],
                EventType = LogisticEnums.EventType.CancelRide,
                Timestamp = DateTime.Now
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity)
            };
            return newEvent;
        }

        public CustomEvent RemovePassenger(string json, Entity entity)
        {
            JObject obj = JObject.Parse(json);

            var addPassengerRideShareEntity = new RideShare
            {
                DriverId = (Guid)   obj["DriverId"],
                Location = (string) obj["Location"],
                EventType = LogisticEnums.EventType.RemovePassenger,
                Timestamp = DateTime.Now
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity)
            };
            return newEvent;
        }

        public CustomEvent StartRide(string json, Entity entity)
        {
            Event newEvent = new Event();
            newEvent.EventType = "StartRide";

            return newEvent;
        }

        public CustomEvent UpdateRide(string json, Entity entity)
        {
            Event newEvent = new Event();
            newEvent.EventType = "UpdateRide";

            return newEvent;

        }
    }
}
