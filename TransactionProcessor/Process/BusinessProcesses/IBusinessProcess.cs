using System.Collections.Generic;
using SharedObjects.Logistic;

namespace TransactionProcessor.Process.BusinessProcesses
{
    internal interface IBusinessProcess
    {
        CustomEvent AddEvent(CustomEvent newEvent, List<CustomEvent> eventHistory);
        bool MakeFinal(List<CustomEvent> eventHistory);
        string AcceptInvite(string jsonString, List<Signatory> signatoryList);
    }
}
