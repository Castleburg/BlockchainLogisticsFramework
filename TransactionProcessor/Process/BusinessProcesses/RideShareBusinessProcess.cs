using System;
using SharedObjects.Logistic;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Enums;
using TransactionProcessor.Process.Interfaces;
using TransactionProcessor.Process.ProcessHandler;

namespace TransactionProcessor.Process.BusinessProcesses
{
    internal class RideShareBusinessProcess : IBusinessProcess
    {
        IRidesharingHandler _rideHandler = new RidesharingHandler();

        public CustomEvent AddEvent(AddEvent newEvent, Entity entity)
        {
            var json = newEvent.JsonContainer;

            var resultingEvent = newEvent.Type switch
            {
                LogisticEnums.EventType.AddPassenger     => _rideHandler.AddPassenger(json, entity),
                LogisticEnums.EventType.RemovePassenger  => _rideHandler.RemovePassnger(json, entity),
                LogisticEnums.EventType.StartRide        => _rideHandler.StartRide(json, entity),
                LogisticEnums.EventType.CancelRide       => _rideHandler.CancelRide(json, entity),
                LogisticEnums.EventType.UpdateRide       => _rideHandler.UpdateRide(json, entity),

                _ => throw new InvalidTransactionException($"Unknown ActionType {newEvent.Type}")
            };

            return resultingEvent;
        }

        public bool MakeFinal(AddEvent newEvent, Entity entity)
        {

            //If previous instance was not true? set to true
            //If previous instance is true, also set to true? I guess, but don't push an update?
            if(!entity.Final || entity.Final)
                return true;
            
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
