using UnityEngine;

namespace Merlin.AOKore.API
{
    public class Vector3
    {
        public Vector3()
        {
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Zero => new Vector3(0f, 0f, 0f);
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public static Vector3 DistanceAdjust(Vector3 playerLocation, Vector3 targetLocation, float distance)
        {
            Vector3 vector = Normalize(new Vector3(playerLocation.X - targetLocation.X, playerLocation.Y - targetLocation.Y, playerLocation.Z - targetLocation.Z), distance);
            return new Vector3(targetLocation.X + vector.X, targetLocation.Y + vector.Y, targetLocation.Z + vector.Z);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Vector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
        }

        public static Vector3 Normalize(Vector3 instance, float size = 1f)
        {
            float dist = instance.Distance(Zero);
            return new Vector3(instance.X / dist * size, instance.Y / dist * size, instance.Z / dist * size);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return (a == null && b == null) || (a != null && b != null && (a.X == b.X && a.Y == b.Y) && a.Z == b.Z);
        }

        public float Distance(Vector3 vector)
        {
            return Mathf.Sqrt((X - vector.X) * (X - vector.X) + (Y - vector.Y) * (Y - vector.Y) + (Z - vector.Z) * (Z - vector.Z));
        }

        public float DistanceXZ(Vector3 vector)
        {
            return Mathf.Sqrt((X - vector.X) * (X - vector.X) + (Z - vector.Z) * (Z - vector.Z));
        }
        public override string ToString()
        {
            return string.Format("[X:{0}; Y:{1}; Z:{2}]", X, Y, Z);
        }
    }
}