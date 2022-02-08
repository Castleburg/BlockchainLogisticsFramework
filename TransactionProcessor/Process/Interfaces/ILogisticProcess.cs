using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Logistic;

namespace TransactionProcessor.Process.Interfaces
{
    internal interface ILogisticProcess
    {
        Entity NewEntity(Command command, TransactionContext context);
        Entity AddEvent(Command command, TransactionContext context);
        Entity MakeFinal(Command command, TransactionContext context);
        Entity NewInvite(Command command, TransactionContext context);
        Entity CancelInvite(Command command, TransactionContext context);
        Entity RejectInvite(Command command, TransactionContext context);
        Entity AcceptInvite(Command command, TransactionContext context);
    }
}
