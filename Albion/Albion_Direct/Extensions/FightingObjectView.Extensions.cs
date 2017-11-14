
namespace Albion_Direct
{
    public static class FightingObjectViewExtensions
    {
        public static FightingObjectView GetAttackTarget(this FightingObjectView view) => view.GetFightingObject().GetAttackTarget();

        public static FightingObject GetFightingObject(this FightingObjectView view) => view.FightingObject;

        public static long GetTargetId(this FightingObjectView view) => view.GetFightingObject().GetTargetId();
    }
}