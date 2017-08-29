using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.Worldmap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private WorldPathingRequest _worldPathingRequest;
        private ClusterPathingRequest _bankPathingRequest;
        private bool _isDepositing;

        public void Bank()
        {
            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!HandleMounting(Vector3.zero))
                return;

            if (!_isDepositing && _localPlayerCharacterView.GetLoadPercent() <= _percentageForBanking)
            {
                Core.Log("[Restart]");
                _state.Fire(Trigger.Restart);
                return;
            }

            if (_worldPathingRequest != null)
            {
                if (_worldPathingRequest.IsRunning)
                {
                    if (!HandleMounting(Vector3.zero))
                        return;

                    _worldPathingRequest.Continue();
                }
                else
                {
                    _worldPathingRequest = null;
                }
                return;
            }

            if (_bankPathingRequest != null)
            {
                if (_bankPathingRequest.IsRunning)
                {
                    if (!HandleMounting(Vector3.zero))
                        return;

                    _bankPathingRequest.Continue();
                }
                else
                {
                    _bankPathingRequest = null;
                }
                return;
            }

            API.Direct.Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;
            ClusterDescriptor bankCluster = townCluster.GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetDestination();

            if (currentWorldCluster.GetName() == bankCluster.GetName())
            {
                var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

                if (banks.Count == 0)
                    return;

                _currentTarget = banks.First();
                if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), _currentTarget, IsBlocked, out List<Vector3> pathing))
                {
                    _bankPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                    return;
                }

                if (_currentTarget is BankBuildingView resource)
                {
                    _localPlayerCharacterView.Interact(resource);
                    //Get inventory
                    var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;

                    var ToDeposit = new List<UIItemSlot>();

                    //Get all items we need
                    var resourceTypes = Enum.GetNames(typeof(ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
                    foreach (var slot in playerStorage.ItemsSlotsRegistered)
                        if (slot != null && slot.ObservedItemView != null)
                        {
                            var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                            if (resourceTypes.Any(r => slotItemName.Contains(r)))
                                ToDeposit.Add(slot);
                        }

                    _isDepositing = ToDeposit != null && ToDeposit.Count > 0;
                    foreach (var item in ToDeposit)
                    {
                        GameGui.Instance.MoveItemToItemContainer(item, vaultStorage.ItemContainerProxy);
                    }
                }

                if (_isDepositing)
                    return;
                else
                {
                    Core.Log("[Bank Done]");
                    _state.Fire(Trigger.BankDone);
                }
            }
            else
            {
                var pathfinder = new WorldmapPathfinder();
                if (pathfinder.TryFindPath(currentWorldCluster, bankCluster, (v) => false, out var path, out var pivots))
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, bankCluster, path);
            }
        }
    }
}