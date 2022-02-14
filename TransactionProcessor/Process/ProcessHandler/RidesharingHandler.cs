using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using SharedObjects.RideShare;
using System.Linq;
using Sawtooth.Sdk.Processor;

namespace TransactionProcessor.Process.ProcessHandler
{
    internal class RidesharingHandler : IRidesharingHandler
    {
        public CustomEvent AddPassenger(RideShare rideShareObj, RideShare latestRideShare)
        {

            if (rideShareObj.DriverId == rideShareObj.PassengerId)
                throw new InvalidTransactionException($"DriverId and PassengerId cannot be the same value");
            if (latestRideShare.PassengerIdList.Contains(rideShareObj.PassengerId))
                throw new InvalidTransactionException($"PassengerId already exists");

            latestRideShare.PassengerIdList.Add(rideShareObj.PassengerId);
            var addPassengerRideShareEntity = new RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.AddPassenger,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };
    
            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity),
                Type = LogisticEnums.EventType.AddPassenger
            };
            return newEvent;
        }

        public CustomEvent RemovePassenger(RideShare rideShareObj, RideShare latestRideShare)
        {
            if (!latestRideShare.PassengerIdList.Remove(rideShareObj.PassengerId))
                throw new InvalidTransactionException($"PassengerId is not in the list");

            var removePassengerRideShareEntity = new RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.RemovePassenger,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(removePassengerRideShareEntity),
                Type = LogisticEnums.EventType.RemovePassenger

            };
            return newEvent;
        }

        public CustomEvent CancelRide(RideShare rideShareObj, RideShare latestRideShare)
        {
            if (rideShareObj.DriverId == latestRideShare.DriverId)
                throw new InvalidTransactionException($"Driver Id is invalid");

            var cancelRideEntity = new RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.CancelRide,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(cancelRideEntity),
                Type = LogisticEnums.EventType.CancelRide
            };
            return newEvent;
        }

        public CustomEvent StartRide(RideShare rideShareObj, RideShare latestRideShare)
        {
            var cancelRideEntity = new RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.StartRide,
                PassengerIdList = new List<string>(),
                Status = rideShareObj.Status
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(cancelRideEntity),
                Type = LogisticEnums.EventType.StartRide
            };
            return newEvent;
        }

        public CustomEvent UpdateRide(RideShare rideShareObj, RideShare latestRideShare)
        {
            var updateRideEntity = new RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.UpdateRide,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(updateRideEntity),
                Type = LogisticEnums.EventType.UpdateRide
            };
            return newEvent;
        }

        public void CheckStatus(RideShare rideShareObj)
        {
            if (rideShareObj.Status == RideShareEnums.RideStatus.Cancelled)
                throw new InvalidTransactionException($"Ride has been cancelled, new events cannot be added");
            if (rideShareObj.Status == RideShareEnums.RideStatus.Finished)
                throw new InvalidTransactionException($"Ride has been finished, new events cannot be added");
        }

        public RideShareSignatoryReward GetSignatoryReward(string jsonString)
        {
            RideShareSignatoryReward signatoryReward;
            try
            {
                signatoryReward = JsonConvert.DeserializeObject<RideShareSignatoryReward>(jsonString);
            }
            catch
            {
                throw new InvalidTransactionException($"Could not unpack signatoryReward.");
            }
            if (signatoryReward is null)
                throw new InvalidTransactionException($"JSON string cannot be empty for signatory reward.");
            return signatoryReward;
        }

        public RideShare GetRideShare(string jsonString)
        {
            RideShare rideShare;
            try
            {
                rideShare = JsonConvert.DeserializeObject<RideShare>(jsonString);
            }
            catch
            {
                throw new InvalidTransactionException($"Could not unpack RideShare.");
            }
            if (rideShare is null)
                throw new InvalidTransactionException($"JSON string cannot be empty for RideShare.");
            return rideShare;
        }

    }
}