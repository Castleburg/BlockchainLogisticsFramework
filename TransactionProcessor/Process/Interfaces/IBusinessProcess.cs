using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Logistic;

namespace TransactionProcessor.Process.Interfaces
{
    internal interface IBusinessProcess
    {
        Event AddEvent(Command command);
        bool MakeFinal(Command command);
        string AcceptInvite(Command command);
    }
}
