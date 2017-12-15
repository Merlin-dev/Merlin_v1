using Albion_Direct;
using Merlin.Pathing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        public const int MAXIMUM_FAIL_ATTEMPTS = 60;

        private SimulationObjectView _currentTarget;
        private int _failedFindAttempts;
        private PositionPathingRequest _changeGatheringPathRequest;

        private void Search()
        {
            if (HandleAttackers())
                return;

            var isCurrentCluster = ObjectManager.GetInstance().GetCurrentCluster().GetName() == _selectedGatherCluster;
            var isHomeCluster = ObjectManager.GetInstance().GetCurrentCluster().GetName() == TownClusterNames[_selectedTownClusterIndex];

            if (isCurrentCluster && _allowSiegeCampTreasure && CanUseSiegeCampTreasure && (_localPlayerCharacterView.GetLoadPercent() > _percentageForSiegeCampTreasure))
            {
                Core.Log("Start Siege Camp Treasure Routine");
                _state.Fire(Trigger.StartSiegeCampTreasure);
                return;
            }

            if (_localPlayerCharacterView.GetLoadPercent() > _percentageForBanking)
            {
                Core.Log("Over allowed Weight. Start Banking procedure.");
                _state.Fire(Trigger.Overweight);
                return;
            }
            
            if (_localPlayerCharacterView.GetLocalPlayerCharacter().HasAnyBrokenItem())
            {
                Core.Log("Damaged - Items fell below 10% durability. Head to Repair in home town");
                _state.Fire(Trigger.Damaged);
                return;
            }

            if (isHomeCluster)
            {
                if (_localPlayerCharacterView.GetLocalPlayerCharacter().HasAnyDamagedItem())
                {
                    Core.Log("We are in home town with damaged items. Fix them before going to harvest.");
                    _state.Fire(Trigger.Damaged);
                    return;
                }
            }

            if (!isCurrentCluster)
            {
                Worldmap worldmapInstance = GameGui.Instance.WorldMap;

                Core.Log("[Travel to target cluster]");
                _targetCluster = worldmapInstance.GetCluster(_selectedGatherCluster).Info;
                _state.Fire(Trigger.StartTravelling);
                return;
            }

            if (Loot())
                return;

            if (_currentTarget != null)
            {
                Core.Log("[Blacklisting target]");

                Blacklist(_currentTarget, TimeSpan.FromMinutes(0.5));

                _currentTarget = null;
                _harvestPathingRequest = null;

                return;
            }

            if (IdentifiedTarget(out SimulationObjectView target))
            {
                Core.Log("[Checking Target]");

                _currentTarget = target;
            }

            if (_currentTarget != null && ValidateTarget(_currentTarget))
            {
                Core.Log("[Identified]");

                _changeGatheringPathRequest = null;
                _failedFindAttempts = 0;
                _state.Fire(Trigger.DiscoveredResource);

                return;
            }
            else
            {
                if (HandlePathing(ref _changeGatheringPathRequest))
                    return;

                _failedFindAttempts++;
                if (_failedFindAttempts > MAXIMUM_FAIL_ATTEMPTS)
                {
                    Core.Log($"[Looking for fallback in {_gatheredSpots.Count} objects]");

                    //Remove all fallback points older than 1 hour
                    var entriesToRemove = _gatheredSpots.Where(kvp => !kvp.Value.HarvestDate.HasValue || kvp.Value.HarvestDate.Value.AddHours(1) < DateTime.UtcNow).ToArray();
                    foreach (var entry in entriesToRemove)
                    {
                        Core.Log($"[Removing {entry.Key} from fallback objects. Too old]");
                        _gatheredSpots.Remove(entry.Key);
                    }

                    var validEntries = _gatheredSpots.Where(kvp =>
                    {
                        var info = new GatherInformation(kvp.Value.ResourceType, kvp.Value.Tier, kvp.Value.EnchantmentLevel);
                        return _gatherInformations[info];
                    }).ToArray();

                    Core.Log($"[Found {validEntries.Length} valid fallback objects]");
                    if (validEntries.Length == 0)
                        return;

                    //Select a random fallback point
                    var spotToUse = validEntries[UnityEngine.Random.Range(0, validEntries.Length)];
                    var spot3d = new Vector3(spotToUse.Key.GetX(), _landscape.GetTerrainHeight(spotToUse.Key, out RaycastHit hit), spotToUse.Key.GetY());
                    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), spot3d, IsBlockedGathering, out List<Vector3> pathing))
                    {
                        Core.Log($"Falling back to {spot3d} which should hold {spotToUse.Value.ToString()}. Removing it from fallback objects.");
                        _changeGatheringPathRequest = new PositionPathingRequest(_localPlayerCharacterView, spot3d, pathing);
                    }
                    else
                        Core.Log($"No path to {spot3d} found. Removing it from fallback objects.");
                    
                    _gatheredSpots.Remove(spotToUse.Key);
                    _failedFindAttempts = 0;
                }
            }
        }

        public bool Loot()
        {
            //var silver = _client.GetEntities<SilverObjectView>(s => !s.IsLootProtected()).FirstOrDefault();
            //if (silver != null)
            //{
            //    Core.Log($"[Silver {silver.name}]");
            //    _localPlayerCharacterView.Interact(silver);
            //    return true;
            //}

            var loot = _client.GetEntities<LootObjectView>(l => l.CanLoot()).FirstOrDefault();
            if (loot != null)
            {
                if (ContainKeepers(loot.transform.position))
                {
                    Core.Log($"[Loot in range of Keepers.Add to Blacklist]");
                    Blacklist(loot, TimeSpan.FromMinutes(2));
                    return false;
                }

                var needsInteraction = !GameGui.Instance.LootGui.gameObject.activeSelf && loot.CanBeUsed;

                if (needsInteraction)
                {
                    Core.Log($"[Loot {loot.name}]");

                    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), loot, IsBlockedWithExitCheck, out List<Vector3> pathing))
                    {
                        Core.Log("Path found Move there now");
                        var _lootPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, loot, pathing);
                        var handle = HandlePathing(ref _lootPathingRequest, null);
                    }

                    _localPlayerCharacterView.Interact(loot);
                    return true;
                }
                else
                {
                    var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    var lootStorage = GameGui.Instance.LootGui.YourInventoryStorage;

                    //Get all items
                    var hasItems = lootStorage.ItemsSlotsRegistered.Any(i => i != null && i.ObservedItemView != null);
                    if (hasItems)
                    {
                        foreach (var slot in lootStorage.ItemsSlotsRegistered)
                            if (slot != null && slot.ObservedItemView != null)
                            {
                                Core.Log($"[Looting {slot.name}]");
                                GameGui.Instance.MoveItemToItemContainer(slot, playerStorage.ItemContainerProxy);
                            }
                        return true;
                    }
                    else
                        Core.Log($"[Looting done]");
                }
            }

            return false;
        }

        public bool IdentifiedTarget(out SimulationObjectView target)
        {
            var views = new List<SimulationObjectView>();

            if (_allowMobHunting)
            {
                var hostiles = _client.GetEntities<MobView>(ValidateMob);
                foreach (var h in hostiles)
                    if (h.GetResourceType().HasValue)
                        views.Add(h);
            }

            var resources = _client.GetEntities<HarvestableObjectView>(ValidateHarvestable);
            foreach (var r in resources)
            {
                views.Add(r);
            }
            
            var filteredViews = views.Where(view =>
            {
                if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(view.transform.position))
                    return false;

                if (_skipKeeperPacks && ContainKeepers(view.transform.position))
                    return false;

                if (view is HarvestableObjectView harvestable)
                {
                    var harvestableObject = harvestable.GetHarvestableObject();

                    var resourceType = harvestableObject.GetResourceType().Value;
                    var tier = (Tier)harvestableObject.GetTier();
                    var enchantmentLevel = (EnchantmentLevel)harvestableObject.GetRareState();

                    var info = new GatherInformation(resourceType, tier, enchantmentLevel);

                    return _gatherInformations[info];
                }
                else if (view is MobView mob)
                {
                    var resourceType = mob.GetResourceType().Value;
                    var tier = (Tier)mob.GetTier();
                    var enchantmentLevel = (EnchantmentLevel)mob.GetRareState();

                    var info = new GatherInformation(resourceType, tier, enchantmentLevel);

                    return _gatherInformations[info];
                }
                else
                    return false;
            });

            target = filteredViews.OrderBy((view) =>
            {
                var playerPosition = _localPlayerCharacterView.transform.position;
                var resourcePosition = view.transform.position;

                var score = (resourcePosition - playerPosition).sqrMagnitude;

                if (view is HarvestableObjectView harvestable)
                {
                    var harvestableObject = harvestable.GetHarvestableObject();
                    var rareState = harvestableObject.GetRareState();

                    if (harvestableObject.GetTier() >= 3) score /= (harvestableObject.GetTier() - 1);
                    if (harvestableObject.GetCharges() == harvestableObject.GetMaxCharges()) score /= 2;
                    if (rareState > 0) score /= ((rareState + 1) * (rareState + 1));
                }
                else if (view is MobView mob)
                {
                    var rareState = mob.GetRareState();

                    if (mob.GetTier() >= 3) score /= (mob.GetTier() - 1);
                    //if (mob.GetCurrentCharges() == mob.GetMaxCharges()) score /= 2;
                    if (rareState > 0) score /= ((rareState + 1) * (rareState + 1));
                }

                var yDelta = Math.Abs(_landscape.GetTerrainHeight(playerPosition.c(), out RaycastHit A_1) - _landscape.GetTerrainHeight(resourcePosition.c(), out RaycastHit A_2));

                score += (yDelta * 10f);

                return (int)score;
            }).FirstOrDefault();

            if (target != null)
                Core.Log($"Resource spotted: {target.name}");

            return target != default(SimulationObjectView);
        }

        public bool ContainKeepers(Vector3 location, float additionalOffset = 0)
        {
            var location2d = location.c();
            if (_keeperSpots.Any(k => Point2.Distance(location2d, k) <= _keeperSkipRange + additionalOffset))
                return true;

            return false;
        }

        public bool IsBlockedGathering(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);
            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            if (_currentTarget != null)
            {
                var resourcePosition = new Vector2(_currentTarget.transform.position.x,
                                                    _currentTarget.transform.position.z);
                var resourceDistance = (resourcePosition - location).magnitude;

                if (resourceDistance < (_currentTarget.GetColliderExtents() + _localPlayerCharacterView.GetColliderExtents()))
                    return false;
            }

            var playerLocation = new Vector2(_localPlayerCharacterView.transform.position.x,
                                                _localPlayerCharacterView.transform.position.z);
            var playerDistance = (playerLocation - location).magnitude;

            if (playerDistance < 2f)
                return false;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            return ((cf & 0x01) != 0) || ((cf & 0x02) != 0) || ((cf & 0xFF) != 0);
        }
    }
}