using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Merlin.Networking.Messages
{
    [Serializable]
    public struct LogMessage : IMessage
    {
        public LogLevel Level;
        public string Message;

        public LogMessage(String message, LogLevel level = LogLevel.Info)
        {
            Message = message;
            Level = level;
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
}
