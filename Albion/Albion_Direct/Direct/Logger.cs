namespace Albion_Direct
{
    public delegate void CoreLogDelegate(string message);

    public static class Logger
    {
        static event CoreLogDelegate OnLog = delegate {};

        public static void SetLogCallback(CoreLogDelegate del)
        {
            OnLog = del;
            OnLog("Albion_Direct logger set.");
        }

        public static void Log(string message)
        {
            OnLog(message);
        }
    }
}
