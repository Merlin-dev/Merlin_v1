using Merlin.API;
using Merlin.API.Direct;

namespace Merlin
{
    public static class FightingObjectExtensions
    {
        public static FightingObjectView GetAttackTarget(this FightingObjectView instance)
        {
            var attackTargetId = instance.GetFightingObject().GetTargetId();

            if (attackTargetId > 0)
            {
                ObjectManager manager = ObjectManager.GetInstance();
                if (manager.GetObject(attackTargetId).GetView() is FightingObjectView target)
                    return target;
            }

            return default(FightingObjectView);
        }

        public static bool IsReadyToCast(this FightingObject instance, SpellSlotIndex slot)
        {
            //NOTE: GetEventHandler Generic param. 0 is internal type of CastSpellEventHandler
            CastSpellEventHandler eventHandler = instance.GetEventHandler<au4>();

            if (eventHandler.CastSpellEventHandler_Internal != null && eventHandler.IsReady((byte)slot))
                return true;

            return false;
        }
    }
}