using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Logistic
{
    public class Signatory
    {
        public string Id { get; set; }
        public string JsonContainer { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
