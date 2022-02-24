using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SawtoothClient.Logistic;
using SawtoothClient.Objects;
using SharedObjects.Enums;
using SharedObjects.RideShare;

namespace SawtoothClient.RideSharing
{
    public class RideShareClient
    {
        private readonly ILogisticClient _logisticClient;

        public RideShareClient(ILogisticClient logisticClient)
        {
            _logisticClient = logisticClient;
        }

        public List<TransactionStatus> StartRide(string driverId, string location)
        {
            var entityStatus = _logisticClient.NewEntity(LogisticEnums.EntityType.RideShare);
            
            var rideShareObj = new RideShareStruct
            {
                 DriverId = driverId,
                 Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);

            var eventStatus = _logisticClient.AddEvent(entityStatus.TransactionId, LogisticEnums.EventType.StartRide, json);
            var status = new List<TransactionStatus>{ entityStatus, eventStatus };
            return status;
        }

        public TransactionStatus AddPassenger(string passengerId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                Location = location,
                PassengerId = passengerId
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StartRide, json);

            return eventStatus;
        }

        public TransactionStatus RemovePassenger(string passengerId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                PassengerId = passengerId,
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StartRide, json);

            return eventStatus;
        }

        public TransactionStatus CancelRide(string driverId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                DriverId = driverId,
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StartRide, json);

            return eventStatus;
        }

        public TransactionStatus UpdateRide(string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StartRide, json);

            return eventStatus;
        }

        public TransactionStatus StopRide(string driverId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                DriverId = driverId,
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StartRide, json);

            return eventStatus;
        }

    }
}
