using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
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
        private bool _reachedPointInBetween;

        public void Repair()
        {
            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (HandlePathing(ref _worldPathingRequest))
                return;

            if (HandlePathing(ref _repairFindPathingRequest, () => _client.GetEntities<RepairBuildingView>((x) => { return true; }).Count > 0))
                return;

            if (HandlePathing(ref _repairPathingRequest, null, () => _reachedPointInBetween = true))
                return;

            Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;

            if (currentWorldCluster.GetName() == townCluster.GetName())
            {
                var repairs = _client.GetEntities<RepairBuildingView>((x) => { return true; });

                if (repairs.Count == 0)
                {
                    var exitPositionPoint = _world.GetCurrentCluster().GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetPosition();
                    var exitPosition = new Vector2(exitPositionPoint.GetX(), exitPositionPoint.GetY());
                    var targetPosition = new Vector3(exitPosition.x, 0, exitPosition.y);

                    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetPosition, IsBlockedWithExitCheck, out List<Vector3> pathing))
                    {
                        Core.Log("[No RepairStation found - moving to bank]");
                        _repairFindPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, targetPosition, pathing);
                    }

                    return;
                }

                _currentTarget = repairs.First();
                if (_currentTarget is RepairBuildingView repairer)
                {
                    if (!repairer.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        if (!_reachedPointInBetween)
                        {
                            var repairCollider = repairer.GetComponentsInChildren<Collider>().First(c => c.name.ToLowerInvariant().Contains("clickable"));
                            var repairColliderPosition = new Vector2(repairCollider.transform.position.x, repairCollider.transform.position.z);
                            var exitPositionPoint = _world.GetCurrentCluster().GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetPosition();
                            var exitPosition = new Vector2(exitPositionPoint.GetX(), exitPositionPoint.GetY());
                            var clampedPosition = Vector2.MoveTowards(repairColliderPosition, exitPosition, 10);
                            var targetPosition = new Vector3(clampedPosition.x, 0, clampedPosition.y);

                            Core.Log("[Move closer to RepairStation]");
                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetPosition, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                _repairPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, targetPosition, pathing);
                            }
                        }
                        else
                        {
                            Core.Log("[Start Interacting with RepairStation]");
                            _localPlayerCharacterView.Interact(repairer);
                        }
                    }
                    else
                    {
                        if (_localPlayerCharacterView.GetLocalPlayerCharacter().HasAnyBrokenItem())
                        {
                            if (_localPlayerCharacterView.IsItemRepairing())
                                return;

                            var repairUsage = GameGui.Instance.BuildingUsageAndManagementGui.BuildingUsage;
                            var silverUI = GameGui.Instance.PaySilverDetailGui;

                            if (silverUI.UserData == repairUsage.RepairItemView && silverUI.gameObject.activeInHierarchy)
                            {
                                Core.Log("[Paying silver costs]");
                                silverUI.OnPay();
                                return;
                            }

                            if (repairUsage.gameObject.activeInHierarchy)
                            {
                                Core.Log("[Reparing all]");
                                repairUsage.RepairItemView.OnClickRepairAllButton();
                                return;
                            }

                            Core.Log("[Interact with RepairStation]");
                            _localPlayerCharacterView.Interact(repairer);
                        }
                        else
                        {
                            _reachedPointInBetween = false;
                            Core.Log("[Repair Done]");
                            _state.Fire(Trigger.RepairDone);
                        }
                    }
                }
            }
            else
            {
                var pathfinder = new WorldmapPathfinder();
                if (pathfinder.TryFindPath(currentWorldCluster, townCluster, StopClusterFunction, out var path, out var pivots))
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, townCluster, path, _skipUnrestrictedPvPZones);
            }
        }
    }
}