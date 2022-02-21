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

        public CustomEvent StopRide(RideShareStruct rideShare, RideShareStruct latestRideShare)
        {
            if (rideShare.DriverId == latestRideShare.DriverId)
                throw new InvalidTransactionException("Driver Id is invalid");
            if (latestRideShare.PassengerIdList.Count > 0)
                throw new InvalidTransactionException("The number of passengers left needs to be zero, before a ride can be stopped.");

            var stopRideEntity = new RideShareStruct
            {
                DriverId = latestRideShare.DriverId,
                Location = rideShare.Location,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = RideShareEnums.RideStatus.Finished
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(stopRideEntity),
                Type = LogisticEnums.EventType.StopRide
            };
            return newEvent;
        }

        public CustomEvent AddPassenger(RideShareStruct rideShareObj, RideShareStruct latestRideShare)
        {
            if (rideShareObj.DriverId == rideShareObj.PassengerId)
                throw new InvalidTransactionException("DriverId and PassengerId cannot be the same value");
            if (latestRideShare.PassengerIdList.Contains(rideShareObj.PassengerId))
                throw new InvalidTransactionException("PassengerId already exists");

            latestRideShare.PassengerIdList.Add(rideShareObj.PassengerId);
            var addPassengerRideShareEntity = new RideShareStruct
            {
                DriverId = latestRideShare.DriverId,
                Location = rideShareObj.Location,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = latestRideShare.Status
            };
    
            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(addPassengerRideShareEntity),
                Type = LogisticEnums.EventType.AddPassenger
            };
            return newEvent;
        }

        public CustomEvent RemovePassenger(RideShareStruct rideShareObj, RideShareStruct latestRideShare)
        {
            if (!latestRideShare.PassengerIdList.Remove(rideShareObj.PassengerId))
                throw new InvalidTransactionException("PassengerId is not in the list");

            var removePassengerRideShareEntity = new RideShareStruct
            {
                DriverId = latestRideShare.DriverId,
                Location = rideShareObj.Location,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = latestRideShare.Status
            };

            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(removePassengerRideShareEntity),
                Type = LogisticEnums.EventType.RemovePassenger
            };
            return newEvent;
        }

        public CustomEvent CancelRide(RideShareStruct rideShareObj, RideShareStruct latestRideShare)
        {
            if (rideShareObj.DriverId == latestRideShare.DriverId)
                throw new InvalidTransactionException("Driver Id is invalid");

            var cancelRideEntity = new RideShareStruct
            {
                DriverId = latestRideShare.DriverId,
                Location = latestRideShare.Location,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = RideShareEnums.RideStatus.Cancelled
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(cancelRideEntity),
                Type = LogisticEnums.EventType.CancelRide
            };
            return newEvent;
        }

        public CustomEvent StartRide(RideShareStruct rideShareObj)
        {
            var cancelRideEntity = new RideShareStruct
            {
                DriverId = rideShareObj.DriverId,
                Location = rideShareObj.Location,
                PassengerIdList = new List<string>(),
                Status = RideShareEnums.RideStatus.Ongoing
            };

            CustomEvent newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(cancelRideEntity),
                Type = LogisticEnums.EventType.StartRide
            };
            return newEvent;
        }

        public CustomEvent UpdateRide(RideShareStruct rideShareObj, RideShareStruct latestRideShare)
        {
            var updateRideEntity = new RideShareStruct
            {
                DriverId = latestRideShare.DriverId,
                Location = rideShareObj.Location,
                PassengerIdList = latestRideShare.PassengerIdList,
                Status = latestRideShare.Status
            };

            var newEvent = new CustomEvent
            {
                TimeStamp = DateTime.Now,
                JsonContainer = JsonConvert.SerializeObject(updateRideEntity),
                Type = LogisticEnums.EventType.UpdateRide
            };
            return newEvent;
        }

        public void CheckStatus(RideShareStruct rideShareObj)
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

        public RideShareStruct GetRideShare(string jsonString)
        {
            RideShareStruct rideShare;
            try
            {
                rideShare = JsonConvert.DeserializeObject<RideShareStruct>(jsonString);
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