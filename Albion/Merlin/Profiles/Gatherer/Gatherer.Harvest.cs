using Albion_Direct;
using Merlin.Pathing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;
using Stateless;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const double MeleeAttackRange = 2.5;
        const float InteractRange = 2f; // Don't remount when arriving from killing mob.

        static class PreviousPlayerInfo
        {
            public static float x = 0f;
            public static float z = 0f;
            public static double stuckProtectionRedivertDuration = 3.0d;
            public static int violationCount = 0;
            public static int violationTolerance = 10;
            public static int StuckCount = 0;
        }

        enum HarvestState
        {
            Enter,
            TravelToResource,
            TravelToMob,
            WalkToResource,
            WalkToMob,
            AttackMob,
            HarvestResource,
            HarvestMob,
        }

        enum HarvestTrigger
        {
            StartHarvest,
            StartHarvestingResource,
            StartHarvestingMob,
            StartWalkingToResource,
            StartWalkingToMob,
            StartAttackingMob,
            StartTravelingToResource,
            StartTravelingToMob
        }

        ClusterPathingRequest _harvestPathingRequest;
        StateMachine<HarvestState, HarvestTrigger> _harvestState;

        void HarvestOnStart()
        {
            _harvestState = new StateMachine<HarvestState, HarvestTrigger>(HarvestState.Enter);
            _harvestState.Configure(HarvestState.Enter)
                .OnEntry(() => OnHarvestEnter())
                .PermitReentry(HarvestTrigger.StartHarvest)
                .Permit(HarvestTrigger.StartTravelingToResource, HarvestState.TravelToResource)
                .Permit(HarvestTrigger.StartWalkingToResource, HarvestState.WalkToResource)
                .Permit(HarvestTrigger.StartTravelingToMob, HarvestState.TravelToMob)
                .Permit(HarvestTrigger.StartWalkingToMob, HarvestState.WalkToMob);

            // Resources
            _harvestState.Configure(HarvestState.TravelToResource)
                .OnEntry(() => OnTravelToResourceEnter())
                .Permit(HarvestTrigger.StartWalkingToResource, HarvestState.WalkToResource)
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            _harvestState.Configure(HarvestState.WalkToResource)
                .OnEntry(() => OnWalkToResourceEnter())
                .Permit(HarvestTrigger.StartHarvestingResource, HarvestState.HarvestResource)
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            _harvestState.Configure(HarvestState.HarvestResource)
                .OnEntry(() => OnHarvestResourceEnter())
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            // Mobs
            _harvestState.Configure(HarvestState.TravelToMob)
                .OnEntry(() => OnTravelToMobEnter())
                .Permit(HarvestTrigger.StartWalkingToMob, HarvestState.WalkToMob)
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            _harvestState.Configure(HarvestState.WalkToMob)
                .OnEntry(() => OnWalkToMobEnter())
                .Permit(HarvestTrigger.StartAttackingMob, HarvestState.AttackMob)
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            _harvestState.Configure(HarvestState.AttackMob)
                .OnEntry(() => OnAttackMobEnter())
                .Permit(HarvestTrigger.StartWalkingToResource, HarvestState.WalkToResource)
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);

            _harvestState.Configure(HarvestState.HarvestMob)
                .OnEntry(() => OnHarvestMobEnter())
                .Permit(HarvestTrigger.StartHarvest, HarvestState.Enter);
        }

        void HarvestUpdate()
        {
            if (HandleAttackers())
                return;

            if ((_currentTarget != null ? _currentTarget.name : "none") == "none")
                Profile.UpdateDelay = Profile.DefaultUpdateDelay;

            switch (_harvestState.State)
            {
                case HarvestState.Enter: DoEnter(); break;
                case HarvestState.TravelToResource: DoTravelToResource(); break;
                case HarvestState.WalkToResource: DoWalkToResource(); break;
                case HarvestState.HarvestResource: DoHarvestResource(); break;
                case HarvestState.TravelToMob: DoTravelToMob(); break;
                case HarvestState.WalkToMob: DoWalkToMob(); break;
                case HarvestState.AttackMob: DoAttackMob(); break;
                case HarvestState.HarvestMob: DoHarvestMob(); break;
            }
        }

        void OnHarvestEnter()
        {
            Core.Log("[Harvesting] -- OnHarvestEnter");
            Assert.IsTrue(_currentTarget is HarvestableObjectView || _currentTarget is MobView);

            if (!ValidateTarget(_currentTarget))
            {
                Core.Log("[Harvesting] - Resource Depleted, search for new one.");
                _state.Fire(Trigger.DepletedResource);
                return;
            }

            float distanceToPlayer = (_currentTarget.transform.position - _localPlayerCharacterView.transform.position).magnitude;

            if (_currentTarget is HarvestableObjectView obj)
            {
                if (distanceToPlayer < InteractRange)
                {
                    _harvestState.Fire(HarvestTrigger.StartWalkingToResource);
                }
                else
                {
                    _harvestState.Fire(HarvestTrigger.StartTravelingToResource);
                }
            }
            else if (_currentTarget is MobView)
            {
                if (distanceToPlayer < InteractRange)
                {
                    _harvestState.Fire(HarvestTrigger.StartWalkingToMob);
                }
                else
                {
                    _harvestState.Fire(HarvestTrigger.StartTravelingToMob);
                }
            }
        }

        void DoEnter()
        { }

        #region Resources
        void OnTravelToResourceEnter()
        {
            Core.Log("[Harvesting] -- OnTravelToResourceEnter");

            Assert.IsTrue(_currentTarget is HarvestableObjectView);

            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlockedGathering, out List<Vector3> pathing))
            {
                Core.Log("[Harvesting] - Path found, begin travel to resource.");
                Core.lineRenderer.positionCount = pathing.Count;
                Core.lineRenderer.SetPositions(pathing.ToArray());
                _harvestPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing,
                    UnityEngine.Random.Range(3f, 8f));
            }
            else
            {
                Core.Log("[Harvesting] - Path not found.");
                _state.Fire(Trigger.DepletedResource);
            }
        }

        void DoTravelToResource()
        {
            Core.LogOnce("[Harvesting] -- DoTravelToResource");

            if (StuckProtection())
            {
                _harvestState.Fire(HarvestTrigger.StartHarvest);
                return;
            }

            if (!HandlePathing(ref _harvestPathingRequest))
                _harvestState.Fire(HarvestTrigger.StartWalkingToResource);
        }

        void OnWalkToResourceEnter()
        {
            Core.Log("[Harvesting] -- OnWalkToResourceEnter");

            HarvestableObjectView resource = _currentTarget as HarvestableObjectView;
            if (_localPlayerCharacterView.IsMounted)
                _localPlayerCharacterView.MountOrDismount();

            _localPlayerCharacterView.Interact(resource);
        }

        void DoWalkToResource()
        {
            Core.LogOnce("[Harvesting] -- DoWalkToResource");

            if (_localPlayerCharacterView.IsHarvesting())
            {
                _harvestState.Fire(HarvestTrigger.StartHarvestingResource);
                return;
            }

            if (StuckHelper.IsPlayerStuck(_localPlayerCharacterView))
            {
                Core.Log("[Harvesting] - Player was stuck. Restarting harvesting.");
                _harvestState.Fire(HarvestTrigger.StartHarvest);
            }
        }

        void OnHarvestResourceEnter()
        {
            Core.Log("[Harvesting] -- OnHarvestResourceEnter");

            HarvestableObjectView resource = _currentTarget as HarvestableObjectView;
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

        void DoHarvestResource()
        {
            Core.LogOnce("[Harvesting] -- DoHarvestResource");

            HarvestableObjectView resource = _currentTarget as HarvestableObjectView;
            if (resource.GetHarvestableObject().GetCharges() <= 0)
            {
                Core.Log("[Harvesting] - Resource depleted, move on.");
                _state.Fire(Trigger.DepletedResource);
                return;
            }
        }
        #endregion Resources

        #region Mobs
        void OnTravelToMobEnter()
        {
            Core.Log("[Harvesting] -- OnTravelToMobEnter");

            Assert.IsTrue(_currentTarget is MobView);
            MobView mob = _currentTarget as MobView;

            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, IsBlockedGathering, out List<Vector3> pathing))
            {
                Core.Log("[Harvesting] - Path found, begin travel to mob,");
                Core.lineRenderer.positionCount = pathing.Count;
                Core.lineRenderer.SetPositions(pathing.ToArray());
                _harvestPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
            }
            else
            {
                Core.Log("[Harvesting] - Path not found.");
                _state.Fire(Trigger.DepletedResource);
            }
        }

        void DoTravelToMob()
        {
            Core.LogOnce("[Harvesting] -- DoTravelToMob");

            Assert.IsTrue(_currentTarget is MobView);
            MobView mob = _currentTarget as MobView;

            if (StuckProtection())
            {
                _harvestState.Fire(HarvestTrigger.StartHarvest);
                return;
            }

            Vector3 targetCenter = _currentTarget.transform.position;
            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            float centerDistance = (targetCenter - playerCenter).magnitude;
            var weaponItem = _localPlayerCharacterView.LocalPlayerCharacter.s7().o();
            var isMeleeWeapon = weaponItem == null || weaponItem.bu() == Albion.Common.GameData.AttackType.Melee;
            var attackRange = _localPlayerCharacterView.LocalPlayerCharacter.jz() + mob.Mob.w8().ew();
            var minimumAttackRange = isMeleeWeapon ? MeleeAttackRange : attackRange;
            var isInLoS = _localPlayerCharacterView.IsInLineOfSight(mob);

            if (!HandlePathing(ref _harvestPathingRequest, () => centerDistance <= minimumAttackRange && isInLoS))
                _harvestState.Fire(HarvestTrigger.StartWalkingToMob);
        }

        void OnWalkToMobEnter()
        {
            Core.Log("[Harvesting] -- OnWalkToMobEnter");

            MobView mob = _currentTarget as MobView;
            if (_localPlayerCharacterView.IsMounted)
                _localPlayerCharacterView.MountOrDismount();

            _localPlayerCharacterView.SetSelectedObject(mob);
            _localPlayerCharacterView.AttackSelectedObject();
        }

        void DoWalkToMob()
        {
            Core.LogOnce("[Harvesting] -- DoWalkToMob");

            MobView mob = _currentTarget as MobView;
            if (_localPlayerCharacterView.IsAttacking())
            {
                _harvestState.Fire(HarvestTrigger.StartAttackingMob);
                return;
            }

            if (StuckHelper.IsPlayerStuck(_localPlayerCharacterView))
            {
                Core.Log("[Harvesting] - Player was stuck. Restarting harvesting.");
                _harvestState.Fire(HarvestTrigger.StartHarvest);
            }
        }

        void OnAttackMobEnter()
        {
            Core.Log("[Harvesting] -- OnAttackMobEnter");
        }

        void DoAttackMob()
        {
            Core.Log("[Harvesting] -- DoAttackMob");

            MobView mob = _currentTarget as MobView;
            if (mob.IsDead() && mob.DeadAnimationFinished)
            {
                Core.Log("[Harvesting] - Mob dead.");
                //_state.Fire(Trigger.DepletedResource);
                //_harvestState.Fire(HarvestTrigger.StartHarvest);
                _harvestState.Fire(HarvestTrigger.StartWalkingToResource);
            }
        }

        void OnHarvestMobEnter()
        {
            //Core.Log("[Harvesting] -- OnHarvestMobEnter");

            //MobView mob = _currentTarget as MobView;
            //_localPlayerCharacterView.Interact(mob);
        }

        void DoHarvestMob()
        {
            //Core.LogOnce("[Harvesting] -- DoHarvestMob");
            //MobView mob = _currentTarget as MobView;
            //if (mob.)

            //    _state.Fire(Trigger.DepletedResource);
        }
        #endregion Mobs

        #region Helpers
        bool ValidateHarvestable(HarvestableObjectView resource)
        {
            if (resource == null)
                return false;

            var resourceObject = resource.GetHarvestableObject();
            if (resourceObject == null)
                return false;

            if (!resourceObject.CanLoot(_localPlayerCharacterView)
                || resourceObject.GetCharges() <= 0
                || resourceObject.GetResourceDescriptor().Tier < (int)SelectedMinimumTier)
            {
                return false;
            }

            Vector3 position = resource.transform.position;
            float terrainHeight = _landscape.GetTerrainHeight(position.c(), out RaycastHit hit);

            if (position.y < terrainHeight - 5)
                return false;

            if (_blacklist.ContainsKey(resource))
                return false;

            //Skip if target is inside a kepper pack
            if (_currentTarget != null && _skipKeeperPacks && (ContainKeepers(_currentTarget.transform.position)))
            {
                Core.Log("[Harvesting] - Skipping resource, inside Keeper Pack Range");
                //Blacklist(resource, TimeSpan.FromMinutes(5));
                return false;
            }

            return true;
        }

        bool ValidateMob(MobView mob)
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

        bool ValidateTarget(SimulationObjectView target)
        {
            if (target is HarvestableObjectView resource)
                return ValidateHarvestable(resource);

            if (target is MobView mob)
                return ValidateMob(mob);

            return false;
        }

        bool StuckProtection()
        {
            if (
                    !_localPlayerCharacterView.IsHarvesting()
                    && !_localPlayerCharacterView.IsAttacking()
                    && _localPlayerCharacterView.IsMounted
                    && Mathf.Abs(_localPlayerCharacterView.GetPosition().x - PreviousPlayerInfo.x) < 0.25f
                    && Mathf.Abs(_localPlayerCharacterView.GetPosition().z - PreviousPlayerInfo.z) < 0.25f
                )
            {
                PreviousPlayerInfo.violationCount++;

                if (PreviousPlayerInfo.violationCount
                        >= PreviousPlayerInfo.violationTolerance)
                {
                    _localPlayerCharacterView.CreateTextEffect("[Stuck detected - Resolving]");
                    PreviousPlayerInfo.StuckCount++;
                    if (forceMove())
                    {
                        PreviousPlayerInfo.violationCount = 0;
                        return true;
                    }
                    else
                    {
                        Profile.UpdateDelay = Profile.DefaultUpdateDelay;
                        return false;
                    }
                }
                else
                {
                    Profile.UpdateDelay = Profile.DefaultUpdateDelay;
                    return false;
                }
            }
            else
            {
                PreviousPlayerInfo.violationCount = 0;
            }
            PreviousPlayerInfo.x = _localPlayerCharacterView.GetPosition().x;
            PreviousPlayerInfo.z = _localPlayerCharacterView.GetPosition().z;
            return false;
        }

        bool forceMove()
        {
            if (_localPlayerCharacterView.IsMounted)
            {
                Profile.UpdateDelay = System.TimeSpan.FromSeconds(PreviousPlayerInfo.stuckProtectionRedivertDuration);
                _localPlayerCharacterView.RequestMove(GetUnstuckCoordinates());
                _currentTarget = null;
                _harvestPathingRequest = null;
                return true;
            }
            else
            {
                Profile.UpdateDelay = Profile.DefaultUpdateDelay;
                return false;
            }
        }

        Vector3 GetUnstuckCoordinates()
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
        #endregion Helpers
    }
}
