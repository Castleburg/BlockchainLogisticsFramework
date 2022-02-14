using SharedObjects.Logistic;
using SharedObjects.RideShare;
namespace TransactionProcessor.Process.BusinessProcesses.RideShare
{
    public interface IRideShareHandler
    {
        public CustomEvent AddPassenger(SharedObjects.RideShare.RideShareStruct rideShare, SharedObjects.RideShare.RideShareStruct latestRideShare);
        public CustomEvent RemovePassenger(SharedObjects.RideShare.RideShareStruct rideShare, SharedObjects.RideShare.RideShareStruct latestRideShare);
        public CustomEvent StartRide(SharedObjects.RideShare.RideShareStruct rideShare);
        public CustomEvent CancelRide(SharedObjects.RideShare.RideShareStruct rideShare, SharedObjects.RideShare.RideShareStruct latestRideShare);
        public CustomEvent StopRide(SharedObjects.RideShare.RideShareStruct rideShare, SharedObjects.RideShare.RideShareStruct latestRideShare);
        public CustomEvent UpdateRide(SharedObjects.RideShare.RideShareStruct rideShare, SharedObjects.RideShare.RideShareStruct latestRideShare);
        public void CheckStatus(SharedObjects.RideShare.RideShareStruct rideShareObj);
        public RideShareSignatoryReward GetSignatoryReward(string jsonString);
        public SharedObjects.RideShare.RideShareStruct GetRideShare(string jsonString);

    }
}