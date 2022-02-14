using SharedObjects.Logistic;
using SharedObjects.RideShare;

namespace TransactionProcessor.Process.BusinessProcesses.RideShare
{
    public interface IRideShareHandler
    {
        public CustomEvent AddPassenger(SharedObjects.RideShare.RideShare rideShare, SharedObjects.RideShare.RideShare latestRideShare);
        public CustomEvent RemovePassenger(SharedObjects.RideShare.RideShare rideShare, SharedObjects.RideShare.RideShare latestRideShare);
        public CustomEvent StartRide(SharedObjects.RideShare.RideShare rideShare, SharedObjects.RideShare.RideShare latestRideShare);
        public CustomEvent CancelRide(SharedObjects.RideShare.RideShare rideShare, SharedObjects.RideShare.RideShare latestRideShare);
        public CustomEvent UpdateRide(SharedObjects.RideShare.RideShare rideShare, SharedObjects.RideShare.RideShare latestRideShare);
        public void CheckStatus(SharedObjects.RideShare.RideShare rideShareObj);
        public RideShareSignatoryReward GetSignatoryReward(string jsonString);
        public SharedObjects.RideShare.RideShare GetRideShare(string jsonString);

    }
}