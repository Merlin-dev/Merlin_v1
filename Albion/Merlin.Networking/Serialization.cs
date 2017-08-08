using Merlin.Networking.Messages;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Merlin.Networking
{
    public static class Serialization
    {
        private static BinaryFormatter formatter = new BinaryFormatter();

        public static byte[] Serialize<T>(this T str) where T : IMessage
        {
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, str);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] arr) where T : IMessage
        {
            using (MemoryStream ms = new MemoryStream(arr))
            {
                return (T)formatter.Deserialize(ms);
            }
        }

        public static void Write(this Stream stream, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                stream.Write(0);
                return;
            }

            byte[] text = Encoding.UTF8.GetBytes(data);

            stream.Write(text.Length);


            stream.Write(text, 0, text.Length);
        }

        public static void Write(this Stream stream, int data)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            stream.Write(bytes, 0, 4);
        }

        public static void Write(this Stream stream, byte[] data)
        {
            stream.Write(data.Length);
            stream.Write(data, 0, data.Length);
        }

        public static string ReadString(this Stream stream)
        {
            int length = stream.ReadInt();

            if (length <= 0)
                return string.Empty;

            byte[] text = new byte[length];
            stream.Read(text, 0, text.Length);

            return Encoding.UTF8.GetString(text);
        }

        public static int ReadInt(this Stream stream)
        {
            byte[] array = new byte[4];
            stream.Read(array, 0, 4);
            return BitConverter.ToInt32(array, 0);
        }

        public static byte[] ReadArray(this Stream stream)
        {
            int length = stream.ReadInt();
            byte[] data = new byte[length];
            stream.Read(data, 0, length);
            return data;
        }
    }
}