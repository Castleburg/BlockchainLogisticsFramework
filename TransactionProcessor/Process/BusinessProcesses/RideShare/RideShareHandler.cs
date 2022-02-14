using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sawtooth.Sdk.Processor;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using SharedObjects.RideShare;

namespace TransactionProcessor.Process.BusinessProcesses.RideShare
{
    internal class RideShareHandler : IRideShareHandler
    {
        public CustomEvent AddPassenger(SharedObjects.RideShare.RideShare rideShareObj, SharedObjects.RideShare.RideShare latestRideShare)
        {

            if (rideShareObj.DriverId == rideShareObj.PassengerId)
                throw new InvalidTransactionException("DriverId and PassengerId cannot be the same value");
            if (latestRideShare.PassengerIdList.Contains(rideShareObj.PassengerId))
                throw new InvalidTransactionException("PassengerId already exists");

            latestRideShare.PassengerIdList.Add(rideShareObj.PassengerId);
            var addPassengerRideShareEntity = new SharedObjects.RideShare.RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.AddPassenger,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };
    
            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity),
                Type = LogisticEnums.EventType.AddPassenger
            };
            return newEvent;
        }

        public CustomEvent RemovePassenger(SharedObjects.RideShare.RideShare rideShareObj, SharedObjects.RideShare.RideShare latestRideShare)
        {
            if (!latestRideShare.PassengerIdList.Remove(rideShareObj.PassengerId))
                throw new InvalidTransactionException("PassengerId is not in the list");

            var removePassengerRideShareEntity = new SharedObjects.RideShare.RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.RemovePassenger,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };

            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(removePassengerRideShareEntity),
                Type = LogisticEnums.EventType.RemovePassenger

            };
            return newEvent;
        }

        public CustomEvent CancelRide(SharedObjects.RideShare.RideShare rideShareObj, SharedObjects.RideShare.RideShare latestRideShare)
        {
            if (rideShareObj.DriverId == latestRideShare.DriverId)
                throw new InvalidTransactionException("Driver Id is invalid");

            var cancelRideEntity = new SharedObjects.RideShare.RideShare
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

        public CustomEvent StartRide(SharedObjects.RideShare.RideShare rideShareObj, SharedObjects.RideShare.RideShare latestRideShare)
        {
            var cancelRideEntity = new SharedObjects.RideShare.RideShare
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

        public CustomEvent UpdateRide(SharedObjects.RideShare.RideShare rideShareObj, SharedObjects.RideShare.RideShare latestRideShare)
        {
            var updateRideEntity = new SharedObjects.RideShare.RideShare
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                EventType = LogisticEnums.EventType.UpdateRide,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = rideShareObj.Status
            };

            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(updateRideEntity),
                Type = LogisticEnums.EventType.UpdateRide
            };
            return newEvent;
        }

        public void CheckStatus(SharedObjects.RideShare.RideShare rideShareObj)
        {
            switch (rideShareObj.Status)
            {
                case RideShareEnums.RideStatus.Cancelled:
                    throw new InvalidTransactionException("Ride has been cancelled, new events cannot be added");
                case RideShareEnums.RideStatus.Finished:
                    throw new InvalidTransactionException("Ride has been finished, new events cannot be added");
                case RideShareEnums.RideStatus.Ongoing:
                    break;
            }
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
                throw new InvalidTransactionException("Could not unpack signatoryReward.");
            }
            if (signatoryReward is null)
                throw new InvalidTransactionException("JSON string cannot be empty for signatory reward.");
            return signatoryReward;
        }

        public SharedObjects.RideShare.RideShare GetRideShare(string jsonString)
        {
            SharedObjects.RideShare.RideShare rideShare;
            try
            {
                rideShare = JsonConvert.DeserializeObject<SharedObjects.RideShare.RideShare>(jsonString);
            }
            catch
            {
                throw new InvalidTransactionException("Could not unpack RideShare.");
            }
            if (rideShare is null)
                throw new InvalidTransactionException("JSON string cannot be empty for RideShare.");
            return rideShare;
        }

    }
}