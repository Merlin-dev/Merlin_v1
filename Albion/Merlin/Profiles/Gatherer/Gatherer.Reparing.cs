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
        private bool _movingToRepair = false;

        public void Repair()
        {
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
                if (!moveToTownRepair(currentWorldCluster))
                {
                    return;
                }
                else
                {
                    if (_localPlayerCharacterView.GetLocalPlayerCharacter().HasAnyDamagedItem())
                    {
                        if (!repairItems())
                        {
                            return;
                        }
                    }
                    else
                    {
                        _movingToRepair = false;

                        _localPlayerCharacterView.RequestMove(GetDefaultBankVector(currentWorldCluster.GetName().ToLowerInvariant()));

                        Core.Log("[Repair Done]");
                        _state.Fire(Trigger.RepairDone);
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
                    _localPlayerCharacterView.RequestMove(GetDefaultBankVector(currentCluster.GetName().ToLowerInvariant()));
                return false;
            }
            else
            {
                _currentTarget = repairs.First();

                if (_currentTarget is RepairBuildingView repairer)
                {
                    if (!repairer.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        if (!_movingToRepair)
                        {
                            Core.Log("Repair Station found, but it's not in range. PathFind");

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