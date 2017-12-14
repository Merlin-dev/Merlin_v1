using UnityEngine;
using Albion_Direct;

namespace Merlin
{
    public static class SimulationObjectExtensions
    {
        public static SimulationObjectView GetView(this SimulationObject obj) => GameManager.GetInstance().GetView(obj);

        public static float GetColliderExtents(this SimulationObject obj) => obj.GetView().GetColliderExtents();

        public static bool ColliderContains(this SimulationObject obj, Vector3 location) => obj.GetView().ColliderContains(location);
    }
}