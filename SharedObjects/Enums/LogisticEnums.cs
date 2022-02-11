using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Enums
{
    public class LogisticEnums
    {
        public enum Commands { NewEntity, AddEvent, MakeFinal, NewInvite, CancelInvite, RejectInvite, AcceptInvite }
        public enum InviteStatus { Pending, Cancelled, Rejected, Signed }
        public enum EntityType { RideShare }

        public enum EventType { StartRide, AddPassenger, RemovePassenger, StopRide, CancelRide, UpdateRide}

    }
}
