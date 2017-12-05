using UnityEngine;
using Albion_Direct;

namespace Merlin
{
    public static class SimulationObjectViewExtensions
    {
        public static SimulationObject GetSimulationObject(this SimulationObjectView view) => view.SimulationObject;

        public static float GetColliderExtents(this SimulationObjectView instance)
        {
            if (instance is HarvestableObjectView)
                return 2.0f;

            var collider = instance.GetComponent<Collider>();
            return collider.GetColliderExtents();
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