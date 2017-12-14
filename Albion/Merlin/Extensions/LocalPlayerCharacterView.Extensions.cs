using Albion_Direct;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.Helpers;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

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
                if (entity.IsDead())
                    return false;

                var target = entity.GetAttackTarget();

                if (target != null && target == view)
                    return true;

                return false;
            });

            attacker = entities.FirstOrDefault();

            return attacker != default(FightingObjectView);
        }

        public static bool IsInLineOfSight(this LocalPlayerCharacterView instance, FightingObjectView target)
        {
            var targetPos = target.FightingObject.h1();
            var sightChecker = instance.PlayerCharacter.zb<ayw>();

            return !ObjectManager.GetInstance().ObjectManager_Internal.y().f(sightChecker.n().h1(), targetPos, out var outPoint, 2);
        }

        public static bool RequestMove(this LocalPlayerCharacterView view, Vector3 position) => view.RequestMove(position.c());

        public static void Interact(this LocalPlayerCharacterView instance, WorldObjectView target, string collider = null) => instance.InputHandler.Interact(target, collider);

        public static void CastOnSelf(this LocalPlayerCharacterView instance, CharacterSpellSlot slot) => instance.InputHandler.CastOn(slot, instance);

        public static void CastOn(this LocalPlayerCharacterView instance, CharacterSpellSlot slot, FightingObjectView target) => instance.InputHandler.CastOn(slot, target);

        public static void CastAt(this LocalPlayerCharacterView instance, CharacterSpellSlot slot, Vector3 target) => instance.InputHandler.CastAt(slot, target);

        public static void SetSelectedObject(this LocalPlayerCharacterView instance, SimulationObjectView target) => instance.InputHandler.SetSelectedObject(target);

        public static void AttackSelectedObject(this LocalPlayerCharacterView instance) => instance.InputHandler.AttackCurrentTarget();

        public static void StopAnyActionObject(this LocalPlayerCharacterView instance) => instance.InputHandler.StopAnyAction();

        public static bool TryFindPath(this LocalPlayerCharacterView instance, AStarPathfinder pathfinder, SimulationObjectView target,
                            StopFunction<Vector2> stopFunction, out List<Vector3> results)
        {
            return instance.TryFindPath(pathfinder, target.transform.position, stopFunction, out results);
        }

        public static bool TryFindPath(this LocalPlayerCharacterView instance, AStarPathfinder pathfinder, Vector3 target,
                                            StopFunction<Vector2> stopFunction, out List<Vector3> results)
        {
            results = new List<Vector3>();

            var pivotPoints = new List<Vector2>();
            var path = new List<Vector2>();

            var startLocation = new Vector2((int)instance.transform.position.x, (int)instance.transform.position.z);
            var endLocation = new Vector2((int)target.x, (int)target.z);

            var landscape = GameManager.GetInstance().GetLandscapeManager();

            if (pathfinder.TryFindPath(startLocation, endLocation, stopFunction, out path, out pivotPoints, true))
            {
                foreach (var point in path)
                {
                    results.Add(new Vector3(point.x, landscape.GetTerrainHeight(point.b(), out RaycastHit hit) + 0.5f, point.y));
                }
                return true;
            }
            return false;
        }
    }
}