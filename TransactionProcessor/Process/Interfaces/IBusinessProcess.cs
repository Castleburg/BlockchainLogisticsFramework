using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Logistic;

namespace TransactionProcessor.Process.Interfaces
{
    internal interface IBusinessProcess
    {
        CustomEvent AddEvent(AddEvent newEvent, Entity entity);
        bool MakeFinal(AddEvent newEvent, Entity entity);
        string AcceptInvite(AddEvent newEvent, Entity entity);

    }
}
