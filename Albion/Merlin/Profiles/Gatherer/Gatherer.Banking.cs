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

        private Vector3 bridgewatch = new Vector3((float)-5.25, (float)-3.5, (float)-13.25);
        private Vector3 caerleon = new Vector3((float)15.8, (float)0.5, (float)-26.5);
        private Vector3 fort_sterling = new Vector3((float)-7.5, (float)2.7, (float)1.75);
        private Vector3 lymhurst = new Vector3((float)-20, (float)2.5, (float)-8.25);
        private Vector3 martlock = new Vector3((float)-3.8, (float)0.5, (float)-14.25);
        private Vector3 thetford = new Vector3((float)-10, (float)14, (float)-5);

        private WorldPathingRequest _worldPathingRequest;
        private ClusterPathingRequest _bankPathingRequest;
        private PositionPathingRequest _bankFindPathingRequest;
        private bool _isDepositing;
        private bool _movingToBank = false;

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
            
            if (HandlePathing(ref _worldPathingRequest))
                return;

            if (HandlePathing(ref _bankFindPathingRequest, () => _client.GetEntities<BankBuildingView>((x) => { return true; }).Count > 0))
                return;

            if (HandlePathing(ref _bankPathingRequest, null, () => _reachedPointInBetween = true))
                return;

            Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;

            if (currentWorldCluster.GetName() == townCluster.GetName())
            {
                var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

                if (banks.Count == 0)
                {
                    Core.Log("No Banks found.");
                    if (_localPlayerCharacterView.IsIdle())
                    {
                        Core.Log("Character Idle. Moving to default bank location");
                        Vector3 bankVector = Vector3.zero;
                        if (currentWorldCluster.GetName().ToLowerInvariant().Equals("bridgewatch"))
                            bankVector = bridgewatch;
                        else if (currentWorldCluster.GetName().ToLowerInvariant().Equals("caerleon"))
                            bankVector = caerleon;
                        else if (currentWorldCluster.GetName().ToLowerInvariant().Equals("fort sterling"))
                            bankVector = fort_sterling;
                        else if (currentWorldCluster.GetName().ToLowerInvariant().Equals("lymhurst"))
                            bankVector = lymhurst;
                        else if (currentWorldCluster.GetName().ToLowerInvariant().Equals("martlock"))
                            bankVector = martlock;
                        else if (currentWorldCluster.GetName().ToLowerInvariant().Equals("thetford"))
                            bankVector = thetford;

                        _localPlayerCharacterView.RequestMove(bankVector);
                    }
                    return;
                }

                _currentTarget = banks.First();

                if (_currentTarget is BankBuildingView resource)
                {
                    if (!resource.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        if (!_movingToBank)
                        {
                            Core.Log("Bank found, but it's not in range. Interact with it to move into range.");

                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), _currentTarget, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                _bankPathingRequest = new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);
                                return;
                            }

                            _movingToBank = true;
                            _localPlayerCharacterView.Interact(resource);
                            return;
                        }
                    }
                    else
                    {
                        Core.Log("Begin Banking.");
                        //Get inventory
                        var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                        var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;

                        var ToDeposit = new List<UIItemSlot>();

                        //Get all items we need that are visible. Need to find a way to get all items in player inventory.
                        var resourceTypes = Enum.GetNames(typeof(ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
                        foreach (var slot in playerStorage.ItemsSlotsRegistered)
                            if (slot != null && slot.ObservedItemView != null)
                            {
                                var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                                //All items not including journals
                                if(!slotItemName.Contains("journalitem"))
                                    if (resourceTypes.Any(r => slotItemName.Contains(r)))
                                        ToDeposit.Add(slot);
                                //adding full journals to deposit list
                                if (slotItemName.Contains("journalitem") && slotItemName.Contains("full"))
                                    ToDeposit.Add(slot);
                            }

                        _isDepositing = ToDeposit != null && ToDeposit.Count > 0;
                        foreach (var item in ToDeposit)
                        {
                            GameGui.Instance.MoveItemToItemContainer(item, vaultStorage.ItemContainerProxy);
                        }

                        if (_isDepositing)
                            return;
                        else
                        {
                            _movingToBank = false;
                            Core.Log("[Bank Done]");
                            _state.Fire(Trigger.BankDone);
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