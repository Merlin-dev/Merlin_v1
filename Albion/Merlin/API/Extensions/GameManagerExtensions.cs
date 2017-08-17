using Merlin.API.Direct;
using System;
using System.Collections.Generic;

namespace Merlin
{
    public static class GameManagerExtensions
    {
        public static List<T> GetEntities<T>(this GameManager manager, Func<T, bool> selector) where T : SimulationObjectView
        {
            var list = new List<T>();

            foreach (var entity in ObjectManager.GetInstance().GetObjectMap().Values)
            {
                if (manager.GetView(entity) is T t && selector(t))
                    list.Add(t);
            }

            return list;
        }
    }
}