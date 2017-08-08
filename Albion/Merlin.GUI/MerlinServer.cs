using Merlin.Networking;
using System.Net;

namespace Merlin.GUI
{
    public class MerlinServer
    {
        #region Singleton

        private static MerlinServer _instance;

        public static MerlinServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MerlinServer();
                }
                return _instance;
            }
        }

        #endregion Singleton

        private TcpServer _server;

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