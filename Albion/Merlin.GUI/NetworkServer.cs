using Merlin.Networking;
using Merlin.Networking.Messages;
using System.Net;

namespace Merlin.GUI
{
    public class NetworkServer
    {
        #region Singleton

        private static NetworkServer _instance;

        public static NetworkServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetworkServer();
                }
                return _instance;
            }
        }

        #endregion Singleton

        private TcpServer _server;

        public static void Send(IMessage message)
        {
            Instance._server.Send(message);
        }

        public void Start()
        {
            _server = new TcpServer();
            _server.OnData += _OnData;

            _server.Start(new IPEndPoint(IPAddress.Any, 5555));
        }

        private void _OnData(IPEndPoint remoteIPEP, byte[] data)
        {
            MessageParser.OnReceive(data);
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}