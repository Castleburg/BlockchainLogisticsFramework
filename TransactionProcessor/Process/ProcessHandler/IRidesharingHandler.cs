using SharedObjects.Logistic;
using SharedObjects.RideShare;

namespace TransactionProcessor.Process.ProcessHandler
{
    public interface IRidesharingHandler
    {
        public CustomEvent AddPassenger(RideShare rideShare, RideShare latestRideShare);
        public CustomEvent RemovePassenger(RideShare rideShare, RideShare latestRideShare);
        public CustomEvent StartRide(RideShare rideShare, RideShare latestRideShare);
        public CustomEvent CancelRide(RideShare rideShare, RideShare latestRideShare);
        public CustomEvent UpdateRide(RideShare rideShare, RideShare latestRideShare);
    }
}