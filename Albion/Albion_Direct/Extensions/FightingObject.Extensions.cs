namespace Albion_Direct
{
    public static class FightingObjectExtensions
    {
        public static FightingObjectView GetAttackTarget(this FightingObject obj)
        {
            var attackTargetId = obj.GetTargetId();

            if (attackTargetId > 0)
            {
                if (GameManager.GetInstance().GetView_Safe(attackTargetId) is FightingObjectView target)
                    return target;
            }

            return default(FightingObjectView);
        }

        public static bool IsReadyToCast(this FightingObject obj, CharacterSpellSlot slot)
        {
            //NOTE: GetEventHandler Generic param. 0 is internal type of CastSpellEventHandler
            CastSpellEventHandler eventHandler = obj.GetEventHandler<ayt>();

            if (eventHandler && eventHandler.IsReady((byte)slot))
                return true;

            return false;
        }
    }
}