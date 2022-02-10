using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Logistic
{
    public class Signature
    {
        public byte[] SignedHashedCommand { get; set; }
        public byte[] HashedCommand { get; set; }
    }
}
