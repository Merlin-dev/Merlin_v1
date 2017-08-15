using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class FightingObjectExtensions
    {
        public static FightingObjectView GetAttackTarget(this FightingObjectView instance)
        {
            var attackTargetId = instance.GetFightingObject().GetTargetId();

            if(attackTargetId > 0)
            {
                ObjectManager manager = ObjectManager.GetInstance();
                if (manager.GetObject(attackTargetId).GetView() is FightingObjectView target)
                    return target;
            }

            return default(FightingObjectView);
        }
    }
}
