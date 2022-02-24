using System;
using System.Collections.Generic;
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
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.AddPassenger, json);

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
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.RemovePassenger, json);

            return eventStatus;
        }

        public List<TransactionStatus> CancelRide(string driverId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                DriverId = driverId,
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.CancelRide, json);

            var finalStatus = _logisticClient.MakeFinal(transactionId);
            var status = new List<TransactionStatus> { eventStatus, finalStatus };

            return status;
        }

        public TransactionStatus UpdateRide(string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.UpdateRide, json);

            return eventStatus;
        }

        public List<TransactionStatus> StopRide(string driverId, string location, Guid transactionId)
        {
            var rideShareObj = new RideShareStruct
            {
                DriverId = driverId,
                Location = location
            };
            var json = JsonConvert.SerializeObject(rideShareObj);
            var eventStatus = _logisticClient.AddEvent(transactionId, LogisticEnums.EventType.StopRide, json);

            var finalStatus = _logisticClient.MakeFinal(transactionId);
            var status = new List<TransactionStatus> { eventStatus, finalStatus };
            return status;
        }

        public TransactionStatus NewInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            return _logisticClient.NewInvite(transactionId, invitedPublicKey);
        }

        public TransactionStatus CancelInvite(Guid transactionId, byte[] invitedPublicKey)
        {
            return _logisticClient.CancelInvite(transactionId, invitedPublicKey);
        }

        public TransactionStatus RejectInvite(Guid transactionId)
        {
            return _logisticClient.RejectInvite(transactionId);
        }

        public TransactionStatus AcceptInvite(string location, DateTime expirationDate, int grantedMinutes, Guid transactionId)
        {
            var rewardObj = new RideShareSignatoryReward
            {
                Location = location,
                ExpirationDate = expirationDate,
                GrantedTime = grantedMinutes
            };
            var json = JsonConvert.SerializeObject(rewardObj);
            var eventStatus = _logisticClient.AcceptInvite(transactionId, json);

            return eventStatus;
        }
    }
}
