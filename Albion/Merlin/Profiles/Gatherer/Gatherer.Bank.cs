using Merlin.API;
using Merlin.Pathing.Worldmap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldMap;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Static

        public static int CapacityForBanking = 99;

        #endregion Static

        #region Fields

        private WorldPathingRequest _worldPathingRequest;
        private ClusterPathingRequest _bankPathingRequest;

        #endregion Fields

        #region Methods

        public void Bank()
        {
            if (!_localPlayerCharacterView.IsMounted)
            {
                if (_localPlayerCharacterView.IsMounting())
                    return;

                _localPlayerCharacterView.MountOrDismount();
                return;
            }

            if (_localPlayerCharacterView.GetLoadPercent() <= CapacityForBanking)
            {
                _localPlayerCharacterView.CreateTextEffect("[Restart]");
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

            Vector3 playerCenter = _localPlayerCharacterView.transform.position;

            var currentWorldCluster = _world.CurrentCluster;
            var _townCluster = _world.GetCluster(SelectedTownCluster);

            var currentCluster = new Cluster(currentWorldCluster.Info);
            var townCluster = new Cluster(_townCluster.Info);

            Core.Log(string.Format(
                $"CurrentCluster: {currentCluster.InternalName} {currentCluster.InternalName} {currentCluster.InternalName}"));
            Core.Log(string.Format(
                $"CurrentCluster: {townCluster.InternalName} {townCluster.InternalName} {townCluster.InternalName}"));



            if (currentCluster.Name == townCluster.Name)
            {
                var banks = _client.GetEntities<BankBuildingView>((x) => { return true; });

                if (banks.Count == 0)
                    return;

                _currentTarget = banks.First();

                /* Begin moving closer the target. */
                var targetCenter = _currentTarget.transform.position;
                playerCenter = _localPlayerCharacterView.transform.position;

                var centerDistance = (targetCenter - playerCenter).magnitude;
                var minimumDistance = _currentTarget.GetColliderExtents() +
                                      _localPlayerCharacterView.GetColliderExtents() + 1.5f;

                if (centerDistance >= minimumDistance)
                {
                    if (!HandleMounting(targetCenter))
                        return;

                    if (_localPlayerCharacterView.TryFindPath(new ClusterPathfinder(), targetCenter, (v) => false,
                        out List<Vector3> pathing))

                        _bankPathingRequest =
                            new ClusterPathingRequest(_localPlayerCharacterView, _currentTarget, pathing);

                    return;
                }
                //Fixes position, if is slightly invalid and opens UI

                if (_currentTarget is BankBuildingView resource)
                {
                    _localPlayerCharacterView.Interact(resource);
                    //Get inventory
                    var playerStorage = GameGui.Instance.CharacterInfoGui.InventoryItemStorage;
                    var vaultStorage = GameGui.Instance.BankBuildingVaultGui.BankVault.InventoryStorage;

                    var ToDeposit = new List<UIItemSlot>();

                    //Get all items we need
                    foreach (var slot in playerStorage.ItemsSlotsRegistered)
                        if (slot != null && slot.ObservedItemView != null)
                        {
                            var name = slot.ObservedItemView.name;
                            //TODO: Move this as part of new filter to Config
                            if (!name.Contains("JOURNAL") && (name.Contains("_ROCK") || name.Contains("_ORE") ||
                                                              name.Contains("_HIDE") || name.Contains("_WOOD") ||
                                                              name.Contains("_FIBER")))
                                ToDeposit.Add(slot);
                        }

                    foreach (var item in ToDeposit)
                    {
                        GameGui.Instance.MoveItemToItemContainer(item, vaultStorage.ItemContainerProxy);
                    }





                }
            }
            else
            {
                var path = new List<WorldmapCluster>();
                var pivotPoints = new List<WorldmapCluster>();

                var worldPathing = new WorldmapPathfinder();

                if (worldPathing.TryFindPath(currentWorldCluster, _townCluster, (cluster) => false, out path, out pivotPoints, true, false))
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, _townCluster, path);
            }
            //_state.Fire(Trigger.Restart);
            //_localPlayerCharacterView.RequestMove(playerCenter);


            // TODO: If not in town, get request to exit towards town.

            // TODO: Get current cluster, is it town?

            // TODO: Move to bank

            // TODO: Near the bank, bank is open?

            // TODO: Bank is open, move items
        }

        #endregion Methods
    }
}