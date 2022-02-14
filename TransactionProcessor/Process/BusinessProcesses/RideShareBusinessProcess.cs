using System;
using SharedObjects.Logistic;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Enums;
using TransactionProcessor.Process.Interfaces;
using TransactionProcessor.Process.ProcessHandler;
using SharedObjects.RideShare;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace TransactionProcessor.Process.BusinessProcesses
{
    internal class RideShareBusinessProcess : IBusinessProcess
    {
        readonly IRidesharingHandler _rideHandler = new RidesharingHandler();

        public CustomEvent AddEvent(CustomEvent newEvent, List<CustomEvent> eventHistory)
        {
            var json = newEvent.JsonContainer;

            var rideShareObj = _rideHandler.GetRideShare(json);
            var latestEvent  = eventHistory.Last();
            var lastRideShareObj = _rideHandler.GetRideShare(latestEvent.JsonContainer);

            _rideHandler.CheckStatus(rideShareObj);

            var resultingEvent = newEvent.Type switch
            {
                LogisticEnums.EventType.AddPassenger => _rideHandler.AddPassenger(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.RemovePassenger => _rideHandler.RemovePassenger(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.StartRide => _rideHandler.StartRide(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.CancelRide => _rideHandler.CancelRide(rideShareObj, lastRideShareObj),
                LogisticEnums.EventType.UpdateRide => _rideHandler.UpdateRide(rideShareObj, lastRideShareObj),

                _ => throw new InvalidTransactionException($"Unknown ActionType {newEvent.Type}")
            };

            return resultingEvent;
        }      

        public bool MakeFinal(CustomEvent newEvent, List<CustomEvent> eventHistory)
        {
            var latestEvent = eventHistory.Last();
            var lastRideShareObj = JsonConvert.DeserializeObject<RideShare>(latestEvent.JsonContainer);

            var rideCompleted = lastRideShareObj.Status == RideShareEnums.RideStatus.Cancelled ||
                                lastRideShareObj.Status == RideShareEnums.RideStatus.Finished;
            return rideCompleted;
        }

        public string AcceptInvite(string jsonString, List<Signatory> signatoryList)
        {
            if (signatoryList.Count != 0)
                throw new InvalidTransactionException($"For ride sharing, only one signatory is allowed");
            RideShareSignatoryReward signatoryReward = _rideHandler.GetSignatoryReward(jsonString);

            var invalidLocation = string.IsNullOrEmpty(signatoryReward.Location);
            var fromTimeStampDoesNotExist = signatoryReward.From == new DateTime();
            var tillTimeStampDoesNotExist = signatoryReward.Till == new DateTime();

            if (invalidLocation || fromTimeStampDoesNotExist || tillTimeStampDoesNotExist)
                throw new InvalidTransactionException($"The JSON string provided to AcceptInvite had errrenerous fields");
            return jsonString;
        }

        
    }
}
