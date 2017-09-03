using UnityEngine;

namespace Merlin.API.Direct
{
    public static class Point2Extensions
    {
        public static Vector3 ToVector3(this Point2 instance)
        {
            return new Vector3(instance.GetX(), 0, instance.GetY());
        }
    }
}