using Merlin.API.Direct;
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

        public static bool TryFindPath(this LocalPlayerCharacterView instance, AStarPathfinder pathfinder, SimulationObjectView target,
                            StopFunction<Vector2> stopFunction, out List<Vector3> results)
        {
            results = new List<Vector3>();

            if (instance.TryFindPath(pathfinder, target.transform.position, stopFunction, out results))
            {
                /*
				var collider = target.GetComponent<Collider>();
				if (collider != null)
				{
					while (collider.bounds.Contains(results.Last()))
						results.RemoveAt(results.Count - 1);
					var lastNode = results.Last();
					var closestNode = collider.ClosestPointOnBounds(lastNode);
					var direction = (closestNode - lastNode).normalized / 2;
					results.Add(closestNode - direction);
				}
				results.Insert(0, instance.transform.position);
				*/
            }
            else
            {
                return false;
            }

            return true;
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
                    results.Add(new Vector3(point.x, landscape.GetTerrainHeight(point.b(), out RaycastHit hit) + 0.5f, point.y));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}