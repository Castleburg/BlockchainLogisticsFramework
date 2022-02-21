using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Commands;
using TransactionProcessor.Context;

namespace TransactionProcessor.Tools.Interfaces
{
    internal interface ICryptographicService
    {
        bool VerifySignature(Token token, CustomCertificate certificate);
    }
}
