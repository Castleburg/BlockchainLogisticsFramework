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

namespace TransactionProcessor.Process.BusinessProcesses
{
    internal class RideShareBusinessProcess : IBusinessProcess
    {
        readonly IRidesharingHandler _rideHandler = new RidesharingHandler();

        public CustomEvent AddEvent(AddEvent newEvent, Entity entity)
        {
            var json = newEvent.JsonContainer;
            var rideShareObj = JsonConvert.DeserializeObject<RideShare>(json);
            var latestEvent = entity.Events.Last();
            var lastRideShareObj = JsonConvert.DeserializeObject<RideShare>(latestEvent.JsonContainer);

            CheckStatus(rideShareObj);

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

        private static void CheckStatus(RideShare rideShareObj)
        {
            if (rideShareObj.Status == RideShareEnums.RideStatus.Cancelled)
                throw new InvalidTransactionException($"Ride has been cancelled, new events cannot be added");
            if (rideShareObj.Status == RideShareEnums.RideStatus.Finished)
                throw new InvalidTransactionException($"Ride has been finished, new events cannot be added");
        }

        public bool MakeFinal(AddEvent newEvent, Entity entity)
        {
            var latestEvent = entity.Events.Last();
            var lastRideShareObj = JsonConvert.DeserializeObject<RideShare>(latestEvent.JsonContainer);

            if (lastRideShareObj.Status == RideShareEnums.RideStatus.Cancelled ||
                lastRideShareObj.Status == RideShareEnums.RideStatus.Finished)
            {
                if (!entity.Final)
                    return true;
                throw new InvalidTransactionException($"A ride cannot be marked final twice");
            }

            return false;
        }

        public string AcceptInvite(AddEvent newEvent, Entity entity)
        {
            //load data, 
            //tjek den ikke er final?
            //Marker at den skal finalizes?
            throw new NotImplementedException();
        }
    }
}
