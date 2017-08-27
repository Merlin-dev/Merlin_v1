using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.World;
using UnityEngine;
using WorldMap;

namespace Merlin.Profiles.Gatherer
{
    public partial class Gatherer
    {
        #region Fields

        private ClusterDescriptor _targetCluster;
        private WorldPathingRequest _travelPathingRequest;

        #endregion Fields

        #region Methods

        public void Travel()
        {
            if (!HandleMounting(Vector3.zero))
                return;

            if (_travelPathingRequest != null)
            {
                if (_travelPathingRequest.IsRunning)
                {
                    if (!HandleMounting(Vector3.zero))
                        return;

                    _travelPathingRequest.Continue();
                }
                else
                {
                    _travelPathingRequest = null;
                }
                return;
            }

            API.Direct.Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            var currentCluster = _world.GetCurrentCluster();
            if (currentCluster.GetName() == _targetCluster.GetName())
            {
                Core.Log("[Traveling Done]");
                _state.Fire(Trigger.TravellingDone);
                return;
            }
            else
            {
                var worldPathing = new WorldPathfinder();
                if (worldPathing.TryFindPath(currentCluster, _targetCluster, out var path))
                {
                    Core.Log("[Traveling Path found]");
                    _travelPathingRequest = new WorldPathingRequest(currentCluster, _targetCluster, path);
                }
                else
                    Core.Log("[No Traveling Path found]");
                return;
            }
        }

        public bool StopClusterFunction(WorldmapCluster cluster)
        {
            var clusterObj = new ClusterDescriptor(cluster.Info).GetClusterType();
            if (clusterObj.GetUiPvpRules() == UiPvpTypes.Full || clusterObj.GetUiPvpRules() == UiPvpTypes.Black)
                return true;

            return false;
        }

        #endregion Methods
    }
}