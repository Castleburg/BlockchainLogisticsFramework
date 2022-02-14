﻿using SharedObjects.Enums;
using System;

namespace SharedObjects.Logistic
{
    public class Signatory
    {
        public string Id { get; set; }
        public byte[] PublicKey { get; set; }
        public string JsonContainer { get; set; }
        public DateTime SignedAt { get; set; }
    }
}
