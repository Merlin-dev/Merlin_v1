using UnityEngine;

namespace Merlin
{
    public static class ColliderExtensions
    {
        public static float GetColliderExtents(this Collider collider)
        {
            if (collider is SphereCollider sphere)
                return sphere.radius;
            else if (collider is CapsuleCollider capsule)
                return capsule.radius;
            else if (collider is BoxCollider box)
                return (new Vector2(box.size.x, box.size.z) * 0.5f).magnitude;

            return 1.0f;
        }
    }
}