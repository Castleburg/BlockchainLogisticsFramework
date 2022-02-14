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
        public Signature Signature { get; set; }
        public RSAParameters RsaParameters { get; set; }
        public string HashAlgorithm { get; set; }
    }
}
