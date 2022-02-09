using System;
using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Logistic;
using TransactionProcessor.Process.Interfaces;

namespace TransactionProcessor.Process.BusinessProcesses
{
    internal class RideShareBusinessProcess : IBusinessProcess
    {
        public Event AddEvent(Command command)
        {
            throw new NotImplementedException();
        }

        public bool MakeFinal(Command command)
        {
            throw new NotImplementedException();
        }

        public string AcceptInvite(Command command)
        {
            throw new NotImplementedException();
        }
    }
}
