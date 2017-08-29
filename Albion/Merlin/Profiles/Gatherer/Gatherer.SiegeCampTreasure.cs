using Merlin.Pathing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Fields

        private IEnumerator _siegeCampTreasureCoroutine;

        #endregion Fields

        #region Properties

        public bool CanUseSiegeCampTreasure => FindObjectsOfType<SiegeCamp>().Length > 0;

        #endregion Properties

        #region Methods

        public void SiegeCampTreasure()
        {
            StartSiegeCampTreasure();
            if (!_siegeCampTreasureCoroutine.MoveNext())
                EndSiegeCampTreasure();
        }

        private void StartSiegeCampTreasure()
        {
            if (_siegeCampTreasureCoroutine == null)
            {
                Core.Log("[Siege Camp Treasure - Start]");

                _siegeCampTreasureCoroutine = SiegeCampTreasureCoroutine();
            }
        }

        private IEnumerator SiegeCampTreasureCoroutine()
        {
            while (!HandleMounting(Vector3.zero))
                yield return null;

            if (_localPlayerCharacterView.GetLoadPercent() <= _percentageForSiegeCampTreasure)
            {
                Core.Log("[Siege Camp Treasure - Capicity not reached]");
                yield break;
            }

            if (!CanUseSiegeCampTreasure)
            {
                Core.Log("[Siege Camp Treasure - Wrong Cluster]");
                yield break;
            }

            var siegeCamp = FindObjectsOfType<SiegeCamp>().First();
            var pathableSiegeCampPosition = siegeCamp.transform.position - new Vector3(0, 0, 10);
            PositionPathingRequest pathingRequest = null;
            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), pathableSiegeCampPosition, SiegeCampTreasureStopFunction, out List<Vector3> pathing))
                pathingRequest = new PositionPathingRequest(_localPlayerCharacterView, pathableSiegeCampPosition, pathing);

            if (pathingRequest == null)
            {
                Core.Log("[Siege Camp Treasure - No Path found]");
                yield break;
            }

            Core.Log("[Siege Camp Treasure - Moving to Treasure Chest]");
            var positionToReturnAfter = _localPlayerCharacterView.transform.position;
            while (pathingRequest.IsRunning)
            {
                pathingRequest.Continue();
                yield return null;
            }

            var siegeCampObject = _client.GetEntities<SiegeCampObjectView>(t => true).FirstOrDefault();
            if (siegeCampObject == null)
            {
                Core.Log("[Siege Camp Treasure - No Siege Camp found]");
                yield break;
            }

            Core.Log("[Siege Camp Treasure - Store Items in Treasure Chest]");
            while (!GameGui.Instance.BankBuildingVaultGui.gameObject.activeSelf)
            {
                _localPlayerCharacterView.Interact(siegeCampObject, "Inventory");
                yield return null;
            }

            var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
            var siegeCampStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;
            List<UIItemSlot> itemsToDeposit;
            do
            {
                itemsToDeposit = new List<UIItemSlot>();

                var resourceTypes = Enum.GetNames(typeof(ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
                foreach (var slot in playerStorage.ItemsSlotsRegistered)
                    if (slot != null && slot.ObservedItemView != null)
                    {
                        var slotItemName = slot.ObservedItemView.name.ToLowerInvariant();
                        if (resourceTypes.Any(r => slotItemName.Contains(r)))
                            itemsToDeposit.Add(slot);
                    }

                foreach (var item in itemsToDeposit)
                {
                    GameGui.Instance.MoveItemToItemContainer(item, siegeCampStorage.ItemContainerProxy);
                    if (item.GetItemInfo() != null)
                        Core.Log($"[Siege Camp Treasure - Stored {item.GetItemStackSize()}x {item.GetItemInfo().e}]");
                }

                yield return null;
            } while (itemsToDeposit.Count > 0);

            Core.Log("[Siege Camp Treasure - Returning to Farm Spot]");
            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), positionToReturnAfter, SiegeCampTreasureStopFunction, out pathing))
                pathingRequest = new PositionPathingRequest(_localPlayerCharacterView, positionToReturnAfter, pathing);

            while (pathingRequest.IsRunning)
            {
                pathingRequest.Continue();
                yield return null;
            }
        }

        private void EndSiegeCampTreasure()
        {
            if (_siegeCampTreasureCoroutine != null)
            {
                _siegeCampTreasureCoroutine = null;

                Core.Log("[Siege Camp Treasure - Done]");
                _state.Fire(Trigger.OnSiegeCampTreasureDone);
            }
        }

        private bool SiegeCampTreasureStopFunction(Vector2 location)
        {
            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                var location3d = new Vector3(location.x, 0, location.y);
                var meshCollidersAtLocation = Physics.OverlapSphere(location3d, 2.0f).Where(c => c.GetType() == typeof(MeshCollider));

                return meshCollidersAtLocation.Any(c => !c.isTrigger);
            }
            else
                return (((cf & 0x01) != 0) || ((cf & 0x02) != 0));
        }

        #endregion Methods
    }
}