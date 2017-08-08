using Merlin.Networking.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Merlin.Networking
{
    public static class MessageParser
    {
        public static Dictionary<string, Action<byte[]>> Receivers = new Dictionary<string, Action<byte[]>> { };

        public static void RegisterReceiver(Type messageType, Action<byte[]> action)
        {
            string stype = messageType.ToString();
            if (Receivers.ContainsKey(stype))
            {
                Receivers[stype] += action;
            }
            else
            {
                Receivers.Add(stype, action);
            }
        }

        public static void OnReceive(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            string stype = ms.ReadString();

            if (Receivers.ContainsKey(stype))
            {
                Receivers[stype].Invoke(ms.ReadArray());
            }
        }

        public static T Read<T>(byte[] data) where T : IMessage
        {
            MemoryStream stream = new MemoryStream(data);

            stream.ReadString();

            return stream.ReadArray().Deserialize<T>();
        }

        public static byte[] Write(IMessage message)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(message.GetType().ToString()); //Pack type as string
            stream.Write(message.Serialize()); //Serialize structure itself
            return stream.ToArray();
        }
    }
}
