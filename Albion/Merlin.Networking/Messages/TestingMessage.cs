using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Merlin.Networking.Messages
{
    public struct TestingMessage : IMessage
    {
        public int Data;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Test;

        public TestingMessage(int data = 99)
        {
            Data = data;
            Test = "Aloha";
        }
    }
}
