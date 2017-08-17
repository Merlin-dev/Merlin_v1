using Merlin.API.Direct;
using System.Linq;
using UnityEngine;

namespace Merlin
{
    public static class LocalPlayerCharacterViewExtensions
    {
        public static LocalPlayerCharacter GetLocalPlayerCharacter(this LocalPlayerCharacterView view) => view.LocalPlayerCharacter;

        public static float GetLoadPercent(this LocalPlayerCharacterView view)
        {
            var character = view.GetLocalPlayerCharacter();
            return character.GetLoadPercentage() * 2f;
        }

        public static bool IsUnderAttack(this LocalPlayerCharacterView view, out FightingObjectView attacker)
        {
            var entities = GameManager.GetInstance().GetEntities<MobView>((entity) =>
            {
                var target = ((FightingObjectView)entity).GetAttackTarget();

                if (target != null && target == view)
                    return true;

                return false;
            });

            attacker = entities.FirstOrDefault();

            return attacker != default(FightingObjectView);
        }

        public static bool RequestMove(this LocalPlayerCharacterView view, Vector3 position) => view.RequestMove(position.c());

        public static void Interact(this LocalPlayerCharacterView instance, WorldObjectView target) => instance.InputHandler.Interact(target);
    }
}