using Merlin.API.Direct;
using Merlin.Pathing;
using Merlin.Pathing.World;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldMap;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders.AStar;

namespace Merlin.Profiles.Gatherer
{
    public sealed partial class Gatherer
    {
        private const int BANKING_PECTENTAGE = 99;

        private WorldPathingRequest _worldPathingRequest;
        private ClusterPathingRequest _bankPathingRequest;

        public void Bank()
        {
            var player = _localPlayerCharacterView.GetLocalPlayerCharacter();

            if (!_localPlayerCharacterView.IsMounted)
            {
                if (player.GetIsMounting())
                    return;

                _localPlayerCharacterView.MountOrDismount();
                return;
            }
            if (_localPlayerCharacterView.GetLoadPercent() <= BANKING_PECTENTAGE)
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
            ClusterDescriptor townCluster = worldmapInstance.GetCluster("Martlock").Info;
            ClusterDescriptor bankCluster = townCluster.GetExits().Find(e => e.GetDestination().GetName().Contains("Bank")).GetDestination();

            if (currentWorldCluster.GetName() == bankCluster.GetName())
            {
               
            }
            else
            {
                var path = new List<ClusterDescriptor>();

                var pathfinder = new WorldPathfinder();

                if(pathfinder.TryFindPath(currentWorldCluster, bankCluster, out path))
                    _worldPathingRequest = new WorldPathingRequest(currentWorldCluster, bankCluster, path);
            }
        }
    }
}