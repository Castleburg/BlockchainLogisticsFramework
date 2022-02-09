using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Commands;
using TransactionProcessor.Tools.Interfaces;

namespace TransactionProcessor.Tools
{
    public class RsaCryptographicService : ICryptographicService
    {
        public bool VerifySignature(CommandToken token)
        {
            throw new NotImplementedException();
        }
    }
}
