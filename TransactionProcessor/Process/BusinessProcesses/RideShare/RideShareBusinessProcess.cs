using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sawtooth.Sdk.Processor;
using SharedObjects.Enums;
using SharedObjects.Logistic;
using SharedObjects.RideShare;

namespace TransactionProcessor.Process.BusinessProcesses.RideShare
{
    internal class RideShareBusinessProcess : IBusinessProcess
    {
        readonly IRideShareHandler _rideHandler;

        public RideShareBusinessProcess()
        {
            _rideHandler = new RideShareHandler();
        }

        public CustomEvent AddEvent(CustomEvent newEvent, List<CustomEvent> eventHistory)
        {
            var json = newEvent.JsonContainer;
            var rideShareObj = _rideHandler.GetRideShare(json);
            RideShareStruct lastRideShareObj = null;
            if (newEvent.Type != LogisticEnums.EventType.StartRide)
            {
                CustomEvent latestEvent = eventHistory.Last();
                lastRideShareObj = _rideHandler.GetRideShare(latestEvent.JsonContainer);
            }
            _rideHandler.CheckStatus(rideShareObj);

            var resultingEvent = newEvent.Type switch
            {
                LogisticEnums.EventType.StartRide => _rideHandler.StartRide(rideShareObj),
                LogisticEnums.EventType.AddPassenger => _rideHandler.AddPassenger(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.RemovePassenger => _rideHandler.RemovePassenger(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.CancelRide => _rideHandler.CancelRide(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.StopRide => _rideHandler.StopRide(rideShareObj, lastRideShareObj),

                LogisticEnums.EventType.UpdateRide => _rideHandler.UpdateRide(rideShareObj, lastRideShareObj),

                _ => throw new InvalidTransactionException($"Unknown ActionType {newEvent.Type}")
            };

            return resultingEvent;
        }      

        public bool MakeFinal(List<CustomEvent> eventHistory)
        {
            var latestEvent = eventHistory.Last();
            var lastRideShareObj = _rideHandler.GetRideShare(latestEvent.JsonContainer);

            var rideCompleted = lastRideShareObj.Status == RideShareEnums.RideStatus.Cancelled ||
                                lastRideShareObj.Status == RideShareEnums.RideStatus.Finished;
            return rideCompleted;
        }

        public string AcceptInvite(string jsonString, List<Signatory> signatoryList)
        {
            if (signatoryList.Count != 0)
                throw new InvalidTransactionException($"For ride sharing, only one signatory is allowed");
            var signatoryReward = _rideHandler.GetSignatoryReward(jsonString);

            var invalidLocation = string.IsNullOrEmpty(signatoryReward.Location);
            var noGrantedTime = signatoryReward.GrantedTime <= 0;
            var tillTimeStampDoesNotExist = signatoryReward.ExpirationDate == new DateTime();

            if (invalidLocation || noGrantedTime || tillTimeStampDoesNotExist)
                throw new InvalidTransactionException($"The JSON string provided to AcceptInvite had erroneous fields");
            return jsonString;
        }
        
    }
}
