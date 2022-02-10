﻿using System;
using System.Collections.Generic;
using System.Text;
using SharedObjects.Commands;

namespace TransactionProcessor.Tools.Interfaces
{
    internal interface ICryptographicService
    {
        bool VerifySignature(CommandToken token);
    }
}