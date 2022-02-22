using SharedObjects.Commands;

namespace TransactionProcessor.Tools.Interfaces
{
    internal interface ICryptographicService
    {
        bool VerifyToken(Token token);
    }
}
