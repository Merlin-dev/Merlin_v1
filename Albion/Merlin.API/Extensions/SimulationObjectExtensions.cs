using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class SimulationObjectExtensions
    {
        public static SimulationObjectView GetView(this SimulationObject instance)
        {
            return GameManager.GetInstance().GetView(instance);
        }
    }
}
