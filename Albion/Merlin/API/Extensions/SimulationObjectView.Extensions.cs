using Merlin.API.Direct;
using UnityEngine;

namespace Merlin
{
    public static class SimulationObjectViewExtensions
    {
        public static SimulationObject GetSimulationObject(this SimulationObjectView view) => view.SimulationObject;

        public static float GetColliderExtents(this SimulationObjectView instance)
        {
            if (instance is HarvestableObjectView resource)
                return 2.0f;

            var collider = instance.GetComponent<Collider>();

            if (collider is SphereCollider sphere)
                return sphere.radius;
            else if (collider is CapsuleCollider capsule)
                return capsule.radius;
            else if (collider is BoxCollider box)
                return box.size.sqrMagnitude;

            return 1.0f;
        }

        public static bool ColliderContains(this SimulationObjectView instance, Vector3 location)
        {
            var collider = instance.GetComponent<Collider>();
            var bounds = collider.bounds;

            if (bounds.Contains(location))
                return true;

            return false;
        }
    }
}