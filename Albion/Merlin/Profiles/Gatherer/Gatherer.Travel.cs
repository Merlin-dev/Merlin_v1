using Albion_Direct;
using Albion_Direct.Pathing;
using Albion_Direct.Pathing.Worldmap;
using System.Linq;
using UnityEngine;

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

            if (HandlePathing(ref _travelPathingRequest))
                return;

            Albion_Direct.Worldmap worldmapInstance = GameGui.Instance.WorldMap;

            var currentCluster = _world.GetCurrentCluster();
            if (currentCluster.GetName() == _targetCluster.GetName())
            {
                Core.Log("[Traveling Done]");
                _state.Fire(Trigger.TravellingDone);
                return;
            }
            else
            {
                var worldPathing = new WorldmapPathfinder();
                if (worldPathing.TryFindPath(currentCluster, _targetCluster, StopClusterFunction, out var path, out var pivots))
                {
                    Core.Log("[Traveling Path found]");
                    _travelPathingRequest = new WorldPathingRequest(currentCluster, _targetCluster, path, _skipUnrestrictedPvPZones);
                }
                else
                    Core.Log("[No Traveling Path found]");
                return;
            }
        }

        public bool StopClusterFunction(ClusterDescriptor cluster)
        {
            if (_skipRedAndBlackZones)
            {
                var clusterObj = cluster.GetClusterType();
                if (clusterObj.GetUiPvpRules() == UiPvpTypes.Full || clusterObj.GetUiPvpRules() == UiPvpTypes.Black)
                    return true;
            }

            return false;
        }

        public bool IsBlockedWithExitCheck(Vector2 location)
        {
            var vector = new Vector3(location.x, 0, location.y);

            if (_skipUnrestrictedPvPZones && _landscape.IsInAnyUnrestrictedPvpZone(vector))
                return true;

            byte cf = _collision.GetCollision(location.b(), 2.0f);
            if (cf == 255)
            {
                //if the location contains an exit return false (passable), otherwise return true
                var location3d = new Vector3(location.x, 0, location.y);
                var locationContainsExit = Physics.OverlapSphere(location3d, 2.0f).Any(c => c.name.ToLowerInvariant().Equals("exit"));
                return !locationContainsExit;
            }
            else
                return (((cf & 0x01) != 0) || ((cf & 0x02) != 0));
        }

        #endregion Methods
    }
}