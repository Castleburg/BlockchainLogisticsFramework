using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SharedObjects.Logistic;

namespace SharedObjects.Commands
{
    [Serializable]
    public class Token
    {
        public Command Command { get; set; }
        public byte[] SignedCertificate { get; set; }
    }
}
