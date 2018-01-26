using Albion_Direct;
using Albion_Direct.Pathing;
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
        private Vector3 _siegeCampWorldPosition;

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
            _siegeCampWorldPosition = siegeCamp.transform.position;

            var pathableSiegeCampPosition = siegeCamp.transform.position - new Vector3(0, 0, 10);
            PositionPathingRequest pathingRequest = null;
            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), pathableSiegeCampPosition, IsBlockedSiegeCampTreasure, out List<Vector3> pathing))
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

                var resourceTypes = Enum.GetNames(typeof(Albion_Direct.ResourceType)).Select(r => r.ToLowerInvariant()).ToArray();
                var slots = playerStorage.ItemsSlotsRegistered;

                foreach (var slot in slots)
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
            if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), positionToReturnAfter, IsBlockedSiegeCampTreasure, out pathing))
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

        private bool IsBlockedSiegeCampTreasure(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);
            //Filter the siege camp treasure zone out from the unallowed areas list, cause we have to enter it.
            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(pvpZone => Mathf.Pow(_siegeCampWorldPosition.x - pvpZone.k(), 2) + Mathf.Pow(_siegeCampWorldPosition.z - pvpZone.l(), 2) >= Mathf.Pow(pvpZone.m(), 2), vector))
                return true;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            return ((cf & 0x01) != 0) || ((cf & 0x02) != 0) || ((cf & 0xFF) != 0);
        }

        #endregion Methods
    }
}