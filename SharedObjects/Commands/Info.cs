using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;

namespace SharedObjects.Commands
{
    public class Info
    {
        public LogisticEnums.EntityType EntityType { get; set; }
        public LogisticEnums.EventType EventType { get; set; }
        public string JsonContainer { get; set; }
        public byte[] InvitePublicKey { get; set; }
    }
}
