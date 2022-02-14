using Sawtooth.Sdk.Processor;
using SharedObjects.Commands;
using SharedObjects.Logistic;
using System.Collections.Generic;

namespace TransactionProcessor.Process.Interfaces
{
    internal interface IBusinessProcess
    {
        CustomEvent AddEvent(CustomEvent newEvent, List<CustomEvent> eventHistory);
        bool MakeFinal(CustomEvent newEvent, List<CustomEvent> eventHistory);
        string AcceptInvite(string jsonString, List<Signatory> signatoryList);

    }
}
