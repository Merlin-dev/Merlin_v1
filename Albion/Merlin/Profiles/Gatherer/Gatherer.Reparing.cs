using Albion_Direct;
using Albion_Direct.Pathing;
using Albion_Direct.Pathing.Worldmap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private PositionPathingRequest _repairPathingRequest;
        private PositionPathingRequest _repairFindPathingRequest;
        private bool _movingToRepair = false;
        private static DateTime _nextRepairAction;

        public void Repair()
        {
            _client = GameManager.GetInstance();
            if (_client.GetState() != GameState.Playing)
            {
                Core.Log("Client state not equal to Playing so we will wait");
                return;
            }

            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();
            
            if (HandlePathing(ref _worldPathingRequest))
                return;

            if (HandlePathing(ref _repairFindPathingRequest, () => _client.GetEntities<RepairBuildingView>((x) => { return true; }).Count > 0))
                return;

            if (HandlePathing(ref _repairPathingRequest, null))
                return;

            Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;

            if (currentWorldCluster.GetName() == townCluster.GetName())
            {
                Core.Log("Arrived at town");
                if (_nextRepairAction == new DateTime())
                {
                    Core.Log("Adding 3 seconds to Repair wait time to avoid load issues.");
                    _nextRepairAction = DateTime.UtcNow.AddSeconds(3);
                }

                if (waiting(_nextRepairAction))
                    return;

                if (!moveToTownRepair(currentWorldCluster))
                {
                    Core.Log("moving to Town Repair location");
                    return;
                }
                else
                {
                    Core.Log("Begin Repairing.");
                    if (_localPlayerCharacterView.GetLocalPlayerCharacter().HasAnyDamagedItem())
                    {
                        if (!repairItems())
                        {
                            return;
                        }
                    }
                    else
                    {
                        _nextRepairAction = new DateTime();
                        _movingToRepair = false;

                        _localPlayerCharacterView.RequestMove(GetDefaultBankVector(currentWorldCluster.GetName().ToLowerInvariant()));

                        Core.Log("[Repair Done]");
                        _state.Fire(Trigger.RepairDone);
                    }
                }
            }
            else
            {
                Core.Log("Not in town. Try to find path to town.");
                var pathfinder = new WorldmapPathfinder();
                if (pathfinder.TryFindPath(currentWorldCluster, townCluster, StopClusterFunction, out var path, out var pivots))
                {
                    Core.Log("Path Found to Town.");
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, townCluster, path, _skipUnrestrictedPvPZones);
                }
            }
        }

        private bool repairItems()
        {
            if (_localPlayerCharacterView.IsItemRepairing())
                return false;

            var repairUsage = GameGui.Instance.BuildingUsageAndManagementGui.BuildingUsage;
            var silverUI = GameGui.Instance.PaySilverDetailGui;

            if ((silverUI.UserData as RepairItemView) == repairUsage.RepairItemView)
            {
                Core.Log("[Paying silver costs]");
                silverUI.OnPay();
                return false;
            }

            if (repairUsage.gameObject.activeInHierarchy)
            {
                Core.Log("[Reparing all]");
                repairUsage.RepairItemView.OnClickRepairAllButton();
                return false;
            }
            return true;
        }

        private bool moveToTownRepair(ClusterDescriptor currentCluster)
        {
            var repairs = _client.GetEntities<RepairBuildingView>((x) => { return true; });

            if (repairs.Count == 0)
            {
                Core.Log("No Repair Stations found.");
                if (_localPlayerCharacterView.IsIdle())
                {
                    Core.Log("Player is Idle. Moving to Default bank location");
                    _localPlayerCharacterView.RequestMove(GetDefaultBankVector(currentCluster.GetName().ToLowerInvariant()));
                }
                return false;
            }
            else
            {
                Core.Log("Repair Station found.");
                _currentTarget = repairs.First();

                if (_currentTarget is RepairBuildingView repairer)
                {
                    if (!repairer.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        Core.Log("Repair not in range.");
                        if (!_movingToRepair)
                        {
                            Core.Log("Find Repair Collider");

                            var repairCollider = repairer.GetComponentsInChildren<Collider>().First(c => c.name.ToLowerInvariant().Contains("clickable"));
                            var repairColliderPosition = new Vector2(repairCollider.transform.position.x, repairCollider.transform.position.z);
                            var exitPositionPoint = GetDefaultBankVector(currentCluster.GetName().ToLowerInvariant());
                            var exitPosition = new Vector2(exitPositionPoint.x, exitPositionPoint.y);
                            var clampedPosition = Vector2.MoveTowards(repairColliderPosition, exitPosition, 10);
                            var targetPosition = new Vector3(clampedPosition.x, 0, clampedPosition.y);

                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetPosition, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                Core.Log("Path found Move there now");
                                _repairPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, targetPosition, pathing);
                                _movingToRepair = true;
                                return false;
                            }
                        }
                        else
                        {
                            Core.Log("Interact with Repair");
                            _localPlayerCharacterView.Interact(repairer);
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

    }
}