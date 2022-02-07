using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;

namespace SharedObjects.Logistic
{
    public class Invite
    {
        public string PublicKey { get; set; }
        public DateTime CreatedDate { get; set; }
        public string InviteStatus { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
