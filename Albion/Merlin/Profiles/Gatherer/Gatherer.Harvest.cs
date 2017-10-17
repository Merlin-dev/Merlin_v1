using Merlin.Pathing;
using System;
using System.Collections.Generic;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const double MELEE_ATTACK_RANGE = 2.5;

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

            if (_blacklist.ContainsKey(mob))
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

        public void Harvest()
        {
            if (HandleAttackers())
                return;

            if (!ValidateTarget(_currentTarget))
            {
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            if (_currentTarget is HarvestableObjectView harvestableObject)
                HarvestHarvestableObjec(harvestableObject);
            else if (_currentTarget is MobView mob)
                HarvestMob(mob);
        }

        public void HarvestHarvestableObjec(HarvestableObjectView resource)
        {
            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            //Skip if target is inside a kepper pack
            if (_skipKeeperPacks && (ContainKeepers(_currentTarget.transform.position)))
            {
                Core.Log("[Skipped - Inside Kepper Pack Range]");
                Blacklist(resource, TimeSpan.FromHours(24));
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            if (HandlePathing(ref _harvestPathingRequest))
                return;

            var centerDistance = (targetCenter - playerCenter).magnitude;
            var minDistance = _currentTarget.GetColliderExtents() + _localPlayerCharacterView.GetColliderExtents() + 1.5f;

            if (centerDistance >= minDistance)
            {
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlockedGathering, out List<Vector3> pathing))
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

            if (_localPlayerCharacterView.IsHarvesting())
                return;

            if (resource.GetHarvestableObject().GetCharges() <= 0)
            {
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            Core.Log("[Harvesting]");
            _localPlayerCharacterView.Interact(resource);

            var harvestableObject2 = resource.GetHarvestableObject();

            var resourceType = harvestableObject2.GetResourceType().Value;
            var tier = (Tier)harvestableObject2.GetTier();
            var enchantmentLevel = (EnchantmentLevel)harvestableObject2.GetRareState();

            var info = new GatherInformation(resourceType, tier, enchantmentLevel)
            {
                HarvestDate = DateTime.UtcNow
            };

            var position = resource.transform.position.c();
            if (_gatheredSpots.ContainsKey(position))
                _gatheredSpots[position] = info;
            else
                _gatheredSpots.Add(position, info);
        }

        public void HarvestMob(MobView mob)
        {
            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            //Skip if target is inside a kepper pack
            if (_skipKeeperPacks && (ContainKeepers(_currentTarget.transform.position)))
            {
                Core.Log("[Skipped - Inside Kepper Pack Range]");
                Blacklist(mob, TimeSpan.FromHours(24));
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            float centerDistance = (targetCenter - playerCenter).magnitude;

            var weaponItem = _localPlayerCharacterView.LocalPlayerCharacter.s7().o();
            var isMeleeWeapon = weaponItem == null || weaponItem.bu() == Albion.Common.GameData.AttackType.Melee;
            var attackRange = _localPlayerCharacterView.LocalPlayerCharacter.jz() + mob.Mob.w8().ew();

            var minimumAttackRange = isMeleeWeapon ? MELEE_ATTACK_RANGE : attackRange;
            var isInLoS = _localPlayerCharacterView.IsInLineOfSight(mob);

            if (HandlePathing(ref _harvestPathingRequest, () => centerDistance <= minimumAttackRange && isInLoS))
                return;

            if (centerDistance >= minimumAttackRange || !isInLoS)
            {
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlockedGathering, out List<Vector3> pathing))
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