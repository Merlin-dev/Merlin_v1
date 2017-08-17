﻿using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Merlin
{
    public static class SimulationObjectExtensions
    {
        public static SimulationObjectView GetView(this SimulationObject instance)
        {
            return GameManager.GetInstance().GetView(instance);
        }

        public static float GetColliderExtents(this SimulationObject instance) => instance.GetView().GetColliderExtents();
        public static bool ColliderContains(this SimulationObject instance, Vector3 location) => instance.GetView().ColliderContains(location);
    }
}
