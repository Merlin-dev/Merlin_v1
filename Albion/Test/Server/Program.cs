using Merlin.Networking;
using Merlin.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Server
{
    class Program
    {
        private static TcpServer _server;

        static void Main(string[] args)
        {
            //Register receiver
            MessageParser.RegisterReceiver(typeof(TestingMessage), (data) => {
                TestingMessage tm = data.Deserialize<TestingMessage>();
                Console.WriteLine($"[Client] {tm.Message}");
            });

            Run(5555);
        }

        static void Run(ushort port)
        {
            _server = new TcpServer();

            _server.OnStart += _OnStart;
            _server.OnConnect += _OnConnect;
            _server.OnData += _OnData;
            _server.OnDisconnect += _OnDisconnect;
            _server.OnStop += _OnStop;

            var isAlive = true;
            while (isAlive)
            {
                var input = Console.ReadLine();
                var blocks = input.Split(' ');

                switch (blocks[0])
                {
                    default: Console.WriteLine("commands: isListening / start / send <message> / disconnect <ipep> / stop / exit"); break;
                    case "start": _server.Start(new IPEndPoint(IPAddress.Any, port)); break;
                    case "isListening": Console.WriteLine(_server.IsListening ? "server listening" : "server not listening"); break;
                    case "send":
                        {
                            if (blocks.Length < 2)
                            {
                                Console.WriteLine("usage: send <message>");
                            }
                            else
                            {
                                _server.Send(new TestingMessage(blocks[1]));
                            }
                        }
                        break;
                    case "disconnect": _server.Disconnect(blocks[1].ToIPEP()); break;
                    case "stop": _server.Stop(); break;
                    case "exit":
                        {
                            if (_server.IsListening)
                            {
                                _server.Stop();
                            }

                            isAlive = false;
                        }
                        break;
                }
            }
        }

        private static void _OnStart()
        {
            Console.WriteLine("[start] " + _server.LocalIPEP);
        }

        private static void _OnConnect(IPEndPoint remoteIPEP)
        {
            Console.WriteLine("[connect] " + remoteIPEP);
        }

        private static void _OnData(IPEndPoint remoteIPEP, byte[] data)
        {
            MessageParser.OnReceive(data);
        }

        private static void _OnDisconnect(IPEndPoint remoteIPEP, Exception exception)
        {
            Console.WriteLine($"[disconnect] {remoteIPEP} exception: {exception?.Message ?? "null"}");
        }

        private static void _OnStop(Exception exception)
        {
            Console.WriteLine($"[stop] exception: {exception?.Message ?? "null"}");
        }
    }
}
