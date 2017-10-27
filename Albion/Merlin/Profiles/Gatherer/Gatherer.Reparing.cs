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
        private ClusterPathingRequest _repairPathingRequest;
        private PositionPathingRequest _repairFindPathingRequest;
        private bool _reachedPointInBetween;
        private bool _movingToRepair = false;

        public void Repair()
        {
            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!HandleMounting(Vector3.zero))
                return;

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
                    Core.Log("No Repair Stations found.");
                    if (_localPlayerCharacterView.IsIdle())
                    {
                        Core.Log("Character Idle. Moving to default bank location");
                        _localPlayerCharacterView.RequestMove(bankVector);
                    }
                    return;
                }

                _currentTarget = repairs.First();

                if(_currentTarget is RepairBuildingView repairer)
                {
                    if (!repairer.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        if(!_movingToRepair)
                        {
                            Core.Log("Repair Station found, but it's not in range. Interact with it to move into range.");
                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), _currentTarget, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                _repairPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                                return;
                            }

                            _movingToRepair = true;
                            _localPlayerCharacterView.Interact(repairer);
                            return;
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

                            if ((silverUI.UserData as RepairItemView) == repairUsage.RepairItemView)
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
                        }
                        else
                        {
                            _movingToRepair = false;
                            _reachedPointInBetween = false;

                            var mounting = HandleMounting(Vector3.zero);
                            _localPlayerCharacterView.RequestMove(bankVector);

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