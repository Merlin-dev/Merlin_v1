using Merlin.Networking;
using Merlin.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public class NetworkClient
    {
        #region Singleton

        private static NetworkClient _instance;

        public static NetworkClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetworkClient();
                }
                return _instance;
            }
        }

        #endregion Singleton

        private TcpClient _client;

        public static void Send(IMessage message)
        {
            Instance._client.Send(message);
        }

        public NetworkClient()
        {
            _client = new TcpClient();
            _client.OnData += _OnData;
        }

        public void Start()
        {
            _client.Connect("127.0.0.1:5555".ToIPEP());
        }

        private void _OnData(byte[] data)
        {
            MessageParser.OnReceive(data);
        }

        public void Stop()
        {
            _client.Disconnect();
        }

        public static void LogInfo(string message)
        {
            UnityEngine.Debug.Log("Cc: " + _instance._client.IsConnected);
            UnityEngine.Debug.Log("Co: " + _instance._client.IsOpen);
            UnityEngine.Debug.Log("INFO: " + message);
            Send(new LogMessage(message));
        }

        public static void LogWarning(string message)
        {
            Send(new LogMessage(message, LogLevel.Warning));
        }

        public static void LogDebug(string message)
        {
            Send(new LogMessage(message, LogLevel.Debug));
        }

        public static void LogError(string message)
        {
            Send(new LogMessage(message, LogLevel.Error));
        }

        public static void LogException(Exception ex)
        {
            Send(new LogMessage($"Exception was thrown:\n{ex.Message}", LogLevel.Critical));
        }
    }
}
