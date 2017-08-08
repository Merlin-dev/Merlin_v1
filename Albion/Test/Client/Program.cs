using Merlin.Networking;
using Merlin.Networking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Client
{
    class Program
    {
        private static TcpClient _client;

        static void Main(string[] args)
        {
            //Register receiver
            MessageParser.RegisterReceiver(typeof(TestingMessage), (data) => {
                TestingMessage tm = data.Deserialize<TestingMessage>();
                Console.WriteLine($"[Server] {tm.Message}");
            });

            Run(5555);
        }

        private static void Run(ushort port)
        {
            _client = new TcpClient();

            _client.OnConnect += _OnConnect;
            _client.OnData += _OnData;
            _client.OnDisconnect += _OnDisconnect;
            _client.OnOpen += _OnOpen;

            var isAlive = true;
            while (isAlive)
            {
                var input = Console.ReadLine();
                var blocks = input.Split(' ');

                switch (blocks[0])
                {
                    default: Console.WriteLine("commands: connect / send <message> / disconnect / exit"); break;
                    case "connect": _client.Connect("127.0.0.1:5555".ToIPEP()); break;
                    case "send":
                        {
                            if(blocks.Length < 2)
                            {
                                Console.WriteLine("usage: send <message>");
                            }
                            else
                            {
                                _client.Send(new TestingMessage(blocks[1]));
                            }
                        }
                        break;
                    case "disconnect": _client.Disconnect(); break;
                    case "exit":
                        {
                            if (_client.IsConnected)
                            {
                                _client.Disconnect();
                            }
                            isAlive = false;
                        }
                        break;
                }
            }
        }

        private static void _OnOpen()
        {
            Console.WriteLine("[open] Opened connection to server");
        }

        private static void _OnDisconnect(Exception exception = null)
        {
            Console.WriteLine($"[disconnect] exception: {exception?.Message ?? "null"}");
        }

        private static void _OnData(byte[] data)
        {
            MessageParser.OnReceive(data);
        }

        private static void _OnConnect()
        {
            Console.WriteLine("[connect] connected");
        }
    }
}