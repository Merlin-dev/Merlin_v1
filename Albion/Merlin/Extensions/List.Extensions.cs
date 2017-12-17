using System.Collections.Generic;
using System.Linq;

namespace Merlin
{
    public static class ListExtensions
    {
        public static T Back<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);

            return list[list.Count - 1];
        }

        public static void PopBack<T>(this List<T> list)
        {
            if (list.Count == 0)
                return;

            list.RemoveAt(list.Count - 1);
        }

        public static T Front<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);

            return list[0];
        }

        public static void PopFront<T>(this List<T> list)
        {
            if (list.Count == 0)
                return;

            list.RemoveAt(0);
        }

        public static List<T> RepeatedDefault<T>(int count)
        {
            return Repeated(default(T), count);
        }

        public static List<T> Repeated<T>(T value, int count)
        {
            List<T> ret = new List<T>(count);
            ret.AddRange(Enumerable.Repeat(value, count));
            return ret;
        }
    }
}
