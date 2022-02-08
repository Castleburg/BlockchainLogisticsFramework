using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Enums;

namespace SharedObjects.Commands
{
    public class AddEvent
    {
        public LogisticEnums.EventType Type { get; set; }
        public string JsonContainer { get; set; }
    }
}
