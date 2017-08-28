using Merlin.API.Direct;
using Merlin.Pathing;
using System;
using System.Collections.Generic;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const double MINIMUM_ATTACK_RANGE = 10;

        private ClusterPathingRequest _harvestPathingRequest;

        public bool ValidateHarvestable(HarvestableObjectView resource)
        {
            var resourceObject = resource.GetHarvestableObject();

            if (!resourceObject.CanLoot(_localPlayerCharacterView) || resourceObject.GetCharges() <= 0 || resourceObject.GetResourceDescriptor().Tier < (int)SelectedMinimumTier)
                return false;

            Vector3 position = resource.transform.position;
            float terrainHeight = _landscape.GetTerrainHeight(position.c(), out RaycastHit hit);

            if (position.y < terrainHeight - 5)
                return false;

            if (_blacklist.ContainsKey(resource))
                return false;

            return true;
        }

        public bool ValidateMob(MobView mob)
        {
            if (mob.IsDead())
                return false;

            var mobAttackTarget = mob.GetAttackTarget();
            if (mobAttackTarget != null && mobAttackTarget != _localPlayerCharacterView)
                return false;

            return true;
        }

        public bool ValidateTarget(SimulationObjectView target)
        {
            if (target is HarvestableObjectView resource)
                return ValidateHarvestable(resource);

            if (target is MobView mob)
                return ValidateMob(mob);

            return false;
        }

        public bool HandleMounting(Vector3 target)
        {
            LocalPlayerCharacter localPlayer = _localPlayerCharacterView.LocalPlayerCharacter;

            if (!_localPlayerCharacterView.IsMounted)
            {
                if (localPlayer.GetIsMounting())
                    return false;

                MountObjectView mount = _localPlayerCharacterView.GetComponent<MountObjectView>();

                if (mount != null)
                {
                    if (target != Vector3.zero && mount.IsInUseRange(localPlayer))
                        return true;

                    if (mount.IsInUseRange(localPlayer))
                        _localPlayerCharacterView.Interact(mount);
                    else
                        _localPlayerCharacterView.MountOrDismount();
                }
                else
                {
                    _localPlayerCharacterView.MountOrDismount();
                }

                return false;
            }

            return true;
        }

        public void Harvest()
        {
            if (HandleAttackers())
                return;

            if (!ValidateTarget(_currentTarget))
            {
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            var mob = _currentTarget as MobView;
            var resource = _currentTarget as HarvestableObjectView;

            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            float centerDistance = (targetCenter - playerCenter).magnitude;
            var isInLoS = mob != null ? _localPlayerCharacterView.IsInLineOfSight(mob) : true;

            if (_harvestPathingRequest != null)
            {
                if (mob != null && centerDistance <= MINIMUM_ATTACK_RANGE && isInLoS)
                {
                    _harvestPathingRequest = null;
                    return;
                }

                if (_harvestPathingRequest.IsRunning)
                {
                    if (!HandleMounting(Vector3.zero))
                        return;

                    _harvestPathingRequest.Continue();
                }
                else
                {
                    _harvestPathingRequest = null;
                }

                return;
            }

            var minDistance = mob != null ? MINIMUM_ATTACK_RANGE : _currentTarget.GetColliderExtents() + _localPlayerCharacterView.GetColliderExtents() + 1.5f;

            if (centerDistance >= minDistance || !isInLoS)
            {
                if (!HandleMounting(targetCenter))
                    return;

                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlocked, out List<Vector3> pathing))
                {
                    Core.lineRenderer.positionCount = pathing.Count;
                    Core.lineRenderer.SetPositions(pathing.ToArray());
                    _harvestPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                }
                else
                {
                    Core.Log("Path not found");
                    _state.Fire(Trigger.DepletedResource);
                }
                return;
            }

            if (resource != null)
            {
                if (_localPlayerCharacterView.IsHarvesting())
                    return;

                if (resource.GetHarvestableObject().GetCharges() <= 0)
                {
                    _state.Fire(Trigger.DepletedResource);
                    return;
                }

                Core.Log("[Harvesting]");
                _localPlayerCharacterView.Interact(resource);

                var harvestableObject = resource.GetHarvestableObject();

                var resourceType = harvestableObject.GetResourceType().Value;
                var tier = (Tier)harvestableObject.GetTier();
                var enchantmentLevel = (EnchantmentLevel)harvestableObject.GetRareState();

                var info = new GatherInformation(resourceType, tier, enchantmentLevel)
                {
                    HarvestDate = DateTime.UtcNow
                };

                var position = resource.transform.position;
                if (_gatheredSpots.ContainsKey(position))
                    _gatheredSpots[position] = info;
                else
                    _gatheredSpots.Add(position, info);
            }
            else if (mob != null)
            {
                if (_localPlayerCharacterView.IsAttacking())
                    return;

                if (mob.IsDead() && mob.DeadAnimationFinished)
                {
                    Core.Log("[Mob Dead]");
                    _state.Fire(Trigger.DepletedResource);
                    return;
                }

                Core.Log("[Attacking]");
                if (_localPlayerCharacterView.IsMounted)
                    _localPlayerCharacterView.MountOrDismount();

                _localPlayerCharacterView.SetSelectedObject(mob);
                _localPlayerCharacterView.AttackSelectedObject();
            }
        }
    }
}