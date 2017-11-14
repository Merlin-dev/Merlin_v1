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
        private WorldPathingRequest _worldPathingRequest;
        private PositionPathingRequest _bankPathingRequest;
        private PositionPathingRequest _bankFindPathingRequest;
        private bool _isDepositing;
        private bool _movingToBank = false;
        private static DateTime _nextBankAction;

        public void Bank()
        {

            _client = GameManager.GetInstance();
            if (_client.GetState() != GameState.Playing)
            {
                Core.Log("Client state not equal to Playing so we will wait");
                return;
            }

            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!HandleMounting(Vector3.zero))
            {
                Core.Log("Handle mounting");
                return;
            }

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

            if (HandlePathing(ref _bankPathingRequest, null))
                return;

            Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            ClusterDescriptor townCluster = worldmapInstance.GetCluster(TownClusterNames[_selectedTownClusterIndex]).Info;

            if (currentWorldCluster.GetName() == townCluster.GetName())
            {
                Core.Log("Arrived at town");

                if (_nextBankAction == new DateTime())
                {
                    Core.Log("Adding 3 seconds to banking wait time to avoid load issues.");
                    _nextBankAction = DateTime.UtcNow.AddSeconds(3);
                }

                if (waiting(_nextBankAction))
                {
                    return;
                }

                if (!moveToTownBank(currentWorldCluster))
                {
                    Core.Log("moving to Town Bank location");
                    return;
                }
                else
                {
                    Core.Log("Begin Banking.");

                    if (moveObjectsToBank())
                        return;
                    else
                    {
                        _nextBankAction = new DateTime();
                        _movingToBank = false;
                        Core.Log("[Bank Done]");
                        _state.Fire(Trigger.BankDone);
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

        private Vector3 GetDefaultBankVector(string cityName)
        {
            ClusterDescriptor currentWorldCluster = _world.GetCurrentCluster();
            Vector3 bankVector = Vector3.zero;

            if (cityName.Equals("bridgewatch"))
                bankVector = new Vector3((float)-2.5, (float)-4.25, -20);
            else if (cityName.Equals("caerleon"))
                bankVector = new Vector3((float)15.8, (float)0.5, (float)-26.5);
            else if (cityName.Equals("fort sterling"))
                bankVector = new Vector3((float)-7.5, (float)2.7, (float)1.75);
            else if (cityName.Equals("lymhurst"))
                bankVector = new Vector3(-20, (float)2.5, (float)-8.25);
            else if (cityName.Equals("martlock"))
                bankVector = new Vector3((float)-3.8, (float)0.5, (float)-14.25);
            else if (cityName.Equals("thetford"))
                bankVector = new Vector3(-10, 14, -5);

            return bankVector;
        }

        private bool moveObjectsToBank()
        {
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
                    if (!slotItemName.Contains("journalitem"))
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
            return _isDepositing;
        }

        private bool moveToTownBank(ClusterDescriptor currentCluster)
        {
            var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

            if (banks.Count == 0)
            {
                Core.Log("No Banks found.");
                if (_localPlayerCharacterView.IsIdle())
                {
                    Core.Log("Player is Idle. Moving to Default bank location");
                    _localPlayerCharacterView.RequestMove(GetDefaultBankVector(currentCluster.GetName().ToLowerInvariant()));
                }
                return false;
            }
            else
            {
                Core.Log("Bank found.");
                _currentTarget = banks.First();

                if (_currentTarget is BankBuildingView resource)
                {
                    if (!resource.IsInUseRange(_localPlayerCharacterView.LocalPlayerCharacter))
                    {
                        Core.Log("Bank not in range.");
                        if (!_movingToBank)
                        {
                            Core.Log("Find Bank Collider");

                            var bankCollider = resource.GetComponentsInChildren<Collider>().First(c => c.name.ToLowerInvariant().Contains("clickable"));
                            var bankColliderPosition = new Vector2(bankCollider.transform.position.x, bankCollider.transform.position.z);
                            var exitPositionPoint = GetDefaultBankVector(currentCluster.GetName().ToLowerInvariant());
                            var exitPosition = new Vector2(exitPositionPoint.x, exitPositionPoint.y);
                            var clampedPosition = Vector2.MoveTowards(bankColliderPosition, exitPosition, 10);
                            var targetPosition = new Vector3(clampedPosition.x, 0, clampedPosition.y);

                            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetPosition, IsBlockedWithExitCheck, out List<Vector3> pathing))
                            {
                                Core.Log("Path found Move there now");
                                _bankPathingRequest = new PositionPathingRequest(_localPlayerCharacterView, targetPosition, pathing);
                                _movingToBank = true;
                                return false;
                            }
                        }
                        else
                        {
                            Core.Log("Interact with Bank");
                            _localPlayerCharacterView.Interact(resource);
                        }
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        private bool waiting(DateTime _nextAction)
        {
            if (DateTime.UtcNow < _nextAction)
            {
                return true;
            }
            return false;
        }
    }
}