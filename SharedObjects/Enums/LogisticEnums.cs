using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Enums
{
    public class LogisticEnums
    {
        public enum Commands { NewEntity, AddEvent, Invite, CancelInvite, RejectInvite, Sign }
        public enum InviteStatus { Pending, Cancelled, Rejected, Signed }
        public enum EntityType { RideShare }

        public enum EventType {  }

    }
}
