using SharedObjects.Enums;
using System;

namespace SharedObjects.Logistic
{
    public class Signatory
    {
        public Signatory()
        {

        }

        public Signatory(string id, byte[] publicKey, string jsonContainer, DateTime signedAt)
        {
            Id = id;
            PublicKey = publicKey;
            JsonContainer = jsonContainer;
            SignedAt = signedAt;
        }
        public string Id { get; set; }
        public byte[] PublicKey { get; set; }
        public string JsonContainer { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
