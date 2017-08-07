using Merlin.Networking;
using Merlin.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Program
    {
        static TcpClient _clientTCP;

        static void Main(string[] args)
        {
            _clientTCP = new TcpClient();

            string test = "127.0.0.1:5555";
            _clientTCP.Connect(test.ToIPEP());

            System.Threading.Thread.Sleep(100);

            _clientTCP.Send(new TestingMessage(88));

            _clientTCP.Disconnect();
            _clientTCP = null;
        }
    }
}
