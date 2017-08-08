using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Merlin.Networking.Messages
{
    public struct TestingMessage : IMessage
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Message;

        public TestingMessage(String message)
        {
            Message = message;
        }
    }
}
