using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Logistic
{
    public class SignOff
    {
        public List<Invite> Invites { get; set; }
        public List<Signatory> Signatories { get; set; }
    }
}
