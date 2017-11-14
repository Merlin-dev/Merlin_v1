using UnityEngine;

namespace Albion_Direct
{
    public partial class SimulationObject
    {
        public SimulationObjectView GetView()
        {
            return GameManager.GetInstance().GetView(this);
        }

        public float GetColliderExtents() => GetView().GetColliderExtents();

        public bool ColliderContains(Vector3 location) => GetView().ColliderContains(location);
    }
}