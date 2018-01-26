namespace Albion_Direct
{
    public partial class FightingObject
    {
        public FightingObjectView GetAttackTarget()
        {
            var attackTargetId = GetTargetId();

            if (attackTargetId > 0)
            {
                if (GameManager.GetInstance().GetView_Safe(attackTargetId) is FightingObjectView target)
                    return target;
            }

            return default(FightingObjectView);
        }

        public bool IsReadyToCast(CharacterSpellSlot slot)
        {
            //NOTE: GetEventHandler Generic param. 0 is internal type of CastSpellEventHandler
            CastSpellEventHandler eventHandler = GetEventHandler<ayy>();

            if (eventHandler && eventHandler.IsReady((byte)slot))
                return true;

            return false;
        }
    }
}