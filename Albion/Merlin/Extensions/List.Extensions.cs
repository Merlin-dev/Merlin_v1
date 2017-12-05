using System.Collections.Generic;

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
    }
}
