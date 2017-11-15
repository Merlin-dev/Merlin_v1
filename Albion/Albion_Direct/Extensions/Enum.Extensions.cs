using System;

namespace Albion_Direct
{
    public static class EnumExtensions
    {
        public static bool HasFlag<T>(this T value, T flag) where T : struct, IConvertible
        {
            if (value.GetType() != flag.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }
    }
}