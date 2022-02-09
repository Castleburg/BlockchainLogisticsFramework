using System;
using System.Collections.Generic;
using System.Text;

namespace SharedObjects.Commands
{
    public class CommandToken
    {
        public Command Command { get; set; }
        public byte[] SignedCommand { get; set; }
    }
}
