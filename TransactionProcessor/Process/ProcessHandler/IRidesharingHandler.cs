using SharedObjects.Logistic;
namespace TransactionProcessor.Process.ProcessHandler
{
    public interface IRidesharingHandler
    {
        public CustomEvent AddPassenger(string json, Entity entity);
        public CustomEvent RemovePassenger(string json, Entity entity);
        public CustomEvent StartRide(string json, Entity entity);
        public CustomEvent CancelRide(string json, Entity entity);
        public CustomEvent UpdateRide(string json, Entity entity);
    }
}