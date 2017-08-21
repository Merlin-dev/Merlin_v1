namespace Merlin.API.Direct
{
    public partial class FightingObject
    {
        public FightingObjectView GetAttackTarget()
        {
            var attackTargetId = GetTargetId();

            if (attackTargetId > 0)
            {
                ObjectManager manager = ObjectManager.GetInstance();
                if (manager.GetObject(attackTargetId).GetView() is FightingObjectView target)
                    return target;
            }

            return default(FightingObjectView);
        }

        public bool IsReadyToCast(CharacterSpellSlot slot)
        {
            //NOTE: GetEventHandler Generic param. 0 is internal type of CastSpellEventHandler
            CastSpellEventHandler eventHandler = GetEventHandler<au4>();

            if (eventHandler && eventHandler.IsReady((byte)slot))
                return true;

            return false;
        }
    }
}