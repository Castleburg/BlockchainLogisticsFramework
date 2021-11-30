using System;
using System.Text;
using Google.Protobuf;

namespace SharedObjects
{
    public class JsonContainer
    {
        public ByteString JsonByteString { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public void SetJsonByteString(string json)
        {
            JsonByteString = ByteString.CopyFrom(json, Encoding.UTF8);
        }
    }
}
