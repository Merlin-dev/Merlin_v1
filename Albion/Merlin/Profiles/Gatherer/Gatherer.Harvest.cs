using Albion_Direct;
using Albion_Direct.Pathing;
using System;
using System.Collections.Generic;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const double MELEE_ATTACK_RANGE = 2.5;
        public const double RANGED_ATTACK_RANGE = 12;

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
        
         #region StuckProtection - dTormentedSoul
         /*** StuckProtection BEGIN ***/
         private bool onDebugMode = true;
         private static class previousPlayerInfo
         {
             public static float x = 0f;
             public static float z = 0f;
             public static double stuckProtectionRedivertDuration = 3.0d;
             public static int violationCount = 0;
             public static int violationTolerance = 10;
             public static int StuckCount = 0;
         }
 
         private bool StuckProtection()
         {
             if (
                     !_localPlayerCharacterView.IsHarvesting()
                     && !_localPlayerCharacterView.IsAttacking()
                     && _localPlayerCharacterView.IsMounted
                     && Mathf.Abs(_localPlayerCharacterView.GetPosition().x - previousPlayerInfo.x) < 0.25f
                     && Mathf.Abs(_localPlayerCharacterView.GetPosition().z - previousPlayerInfo.z) < 0.25f
                 )
             {
                 previousPlayerInfo.violationCount++;
 
                 if (previousPlayerInfo.violationCount
                         >= previousPlayerInfo.violationTolerance)
                 {
                     _localPlayerCharacterView.CreateTextEffect("[Stuck detected - Resolving]");
                     previousPlayerInfo.StuckCount++;
                     if (forceMove())
                     {
                         previousPlayerInfo.violationCount = 0;
                         return true;
                     }
                     else
                     {
                         Profile.UpdateDelay = System.TimeSpan.FromSeconds(0.1d);
                         return false;
                     }
                 }
                 else
                 {
                     Profile.UpdateDelay = System.TimeSpan.FromSeconds(0.1d);
                     return false;
                 }
             }
             else
             {
                 previousPlayerInfo.violationCount = 0;
             }
             previousPlayerInfo.x = _localPlayerCharacterView.GetPosition().x;
             previousPlayerInfo.z = _localPlayerCharacterView.GetPosition().z;
             return false;
         }
 
         private bool forceMove()
         {
             if (_localPlayerCharacterView.IsMounted)
             {
                 Profile.UpdateDelay = System.TimeSpan.FromSeconds(previousPlayerInfo.stuckProtectionRedivertDuration);
                 _localPlayerCharacterView.RequestMove(GetUnstuckCoordinates());
                 _currentTarget = null;
                 _harvestPathingRequest = null;
                 return true;
             }
             else
             {
                 Profile.UpdateDelay = System.TimeSpan.FromSeconds(0.1d);
                 return false;
             }
         }
 
         private Vector3 GetUnstuckCoordinates()
         {
             var unstuckCoordinates = _localPlayerCharacterView.GetPosition();
             var method = "variable";
             switch (method)
             {
                 case "absolute":
                     float[] arrayValues = { -15f, +15f };
                     unstuckCoordinates.x = _localPlayerCharacterView.GetPosition().x + arrayValues[UnityEngine.Random.Range(0, arrayValues.Length)];
                     unstuckCoordinates.z = _localPlayerCharacterView.GetPosition().z + arrayValues[UnityEngine.Random.Range(0, arrayValues.Length)];
                     break;
                 case "variable":
                     unstuckCoordinates.x = _localPlayerCharacterView.GetPosition().x + (UnityEngine.Random.Range(-1f, +1.01f) * UnityEngine.Random.Range(25f, 55f));
                     unstuckCoordinates.z = _localPlayerCharacterView.GetPosition().z + (UnityEngine.Random.Range(-1f, +1.01f) * UnityEngine.Random.Range(25f, 55f));
                     break;
                 default:
                     break;
             }
             _localPlayerCharacterView.CreateTextEffect("x: " + unstuckCoordinates.x + " | z: " + unstuckCoordinates.z);
             return unstuckCoordinates;
         }
 
         /*** StuckProtection END ***/
         #endregion StuckProtection - dTormentedSoul
 
        public void Harvest()
        {
            if (HandleAttackers())
                return;
            
            #region [dTormentedSoul Area]
            messageDelayIncrement++;
            
            
            if (onDebugMode && (messageDelayIncrement % messageDelayTrigger == 0))
            {
                //_localPlayerCharacterView.CreateTextEffect("Harvest()" + " | " + System.Convert.ToString(_localPlayerCharacterView.GetLocalPlayerCharacter().GetHealth().GetValue()) + "/" + System.Convert.ToString(_localPlayerCharacterView.GetLocalPlayerCharacter().GetHealth().GetMaximum()) + " | " + System.Convert.ToString(previousPlayerInfo.StuckCount) + " | " + (_currentTarget != null ? _currentTarget.name : "none"));
                messageDelayIncrement = 0;
            }
            
            if ((_currentTarget != null ? _currentTarget.name : "none") == "none")
                Profile.UpdateDelay = System.TimeSpan.FromSeconds(0.1d);
            
            if (StuckProtection())
                return;
            
            #endregion [dTormentedSoul Area]

            if (!ValidateTarget(_currentTarget))
            {
                Core.Log("Resource DepletedSearch for new one.");
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            if (_currentTarget is HarvestableObjectView harvestableObject)
            {
                //Core.Log("Begin Harvest of Resource");
                HarvestHarvestableObjec(harvestableObject);
            }
            else if (_currentTarget is MobView mob)
            {
                //Core.Log("Begin Harvest of Mob");
                HarvestMob(mob);
            }
        }

        public void HarvestHarvestableObjec(HarvestableObjectView resource)
        {
            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            //Skip if target is inside a kepper pack
            if (_skipKeeperPacks && (ContainKeepers(_currentTarget.transform.position)))
            {
                Core.Log("[Blacklisted - Inside Kepper Pack Range]");
                Blacklist(resource, TimeSpan.FromMinutes(5));
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
                    Core.Log("Path found, begin travel to resource");
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
            {
                Core.Log("Currently harvesting.Wait until done.");
                return;
            }

            if (resource.GetHarvestableObject().GetCharges() <= 0)
            {
                Core.Log("resource depleted. Move on");
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            Core.Log("[Harvesting] - Interact with resource");
            _localPlayerCharacterView.Interact(resource);

            var harvestableObject2 = resource.GetHarvestableObject();

            var resourceType = harvestableObject2.GetResourceType().Value;
            var tier = (Albion_Direct.Tier)harvestableObject2.GetTier();
            var enchantmentLevel = (Albion_Direct.EnchantmentLevel)harvestableObject2.GetRareState();

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
                Core.Log("[Blacklisted - Inside Kepper Pack Range]");
                Blacklist(mob, TimeSpan.FromMinutes(5));
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            float centerDistance = (targetCenter - playerCenter).magnitude;

            var weaponItem = _localPlayerCharacterView.LocalPlayerCharacter.th().o();
            var isMeleeWeapon = weaponItem == null || weaponItem.bw() == Albion.Common.GameData.AttackType.Melee;
            //var attackRange = _localPlayerCharacterView.LocalPlayerCharacter.t2() + mob.Mob.x1().f9();

            var minimumAttackRange = isMeleeWeapon ? MELEE_ATTACK_RANGE : RANGED_ATTACK_RANGE;
            var isInLoS = _localPlayerCharacterView.IsInLineOfSight(mob);

            if (HandlePathing(ref _harvestPathingRequest, () => centerDistance <= minimumAttackRange && isInLoS))
                return;

            if (centerDistance >= minimumAttackRange || !isInLoS)
            {
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlockedGathering, out List<Vector3> pathing))
                {
                    Core.Log("Path found, begin travel to Mob");
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
            {
                Core.Log("Currently Attacking Mob. Wait until done.");
                return;
            }

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