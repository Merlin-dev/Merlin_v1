using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class LocalPlayerCharacterViewExtensions
    {
        public static LocalPlayerCharacter GetCharacter(this LocalPlayerCharacterView view) => view.LocalPlayerCharacter;

        public static float GetLoadPercent(this LocalPlayerCharacterView view)
        {
            var character = view.GetCharacter();
            return character.GetLoad() / character.GetMaxLoad() * 100f * 2f;
        }

        public static bool IsUnderAttack(this LocalPlayerCharacterView view, out FightingObjectView attacker)
        {
            var entities = GameManager.GetInstance().GetEntities<MobView>((entity) => {
                var target = ((FightingObjectView)entity).GetAttackTarget();

                if (target != null && target == view)
                    return true;

                return false;
            });

            attacker = entities.FirstOrDefault();

            return attacker != default(FightingObjectView);
        }
    }
}
