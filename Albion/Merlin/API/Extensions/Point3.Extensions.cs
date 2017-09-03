using UnityEngine;

namespace Merlin.API.Direct
{
    public static class Point3Extensions
    {
        public static Point2 ToPoint2(this Point3 instance)
        {
            return instance.Point3_Internal.n();
        }
    }
}