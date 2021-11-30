using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;

namespace SharedObjects
{
    public class GeneralistContainer
    {
        public Dictionary<string, JsonContainer> ItemList { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
