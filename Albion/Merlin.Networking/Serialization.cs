using Merlin.Networking.Messages;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Merlin.Networking
{
    public static class Serialization
    {
        public static byte[] Serialize<T>(this T str) where T : IMessage
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static T Deserialize<T>(this byte[] arr) where T : IMessage
        {
            T str = default(T);

            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);

            return str;
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