using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Enums
{
    public class LogisticEnums
    {
        public enum Commands { Undefined, NewEntity, AddEvent, MakeFinal, NewInvite, CancelInvite, RejectInvite, AcceptInvite }
        public enum InviteStatus { Undefined, Pending, Cancelled, Rejected, Signed }
        public enum EntityType { Undefined, RideShare }

        public enum EventType { Undefined, StartRide, AddPassenger, RemovePassenger, StopRide, CancelRide, UpdateRide}

    }
}
