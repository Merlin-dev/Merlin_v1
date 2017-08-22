using System;
using System.Collections.Generic;

namespace Merlin.API.Direct
{
    public partial class GameManager
    {
        public List<T> GetEntities<T>(Func<T, bool> selector) where T : SimulationObjectView
        {
            var list = new List<T>();

            foreach (var entity in ObjectManager.GetInstance().GetObjectMap().Values)
            {
                if (GetView(entity) is T t && selector(t))
                    list.Add(t);
            }

            return list;
        }

        public SimulationObjectView GetView_Safe(long id)
        {
            if (id > 0L)
                return GetView(id);

            return default(SimulationObjectView);
        }
    }
}