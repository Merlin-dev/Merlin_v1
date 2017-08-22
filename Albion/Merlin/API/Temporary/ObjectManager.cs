using System.Collections.Generic;
using WorldMap;

namespace Merlin.API.Direct
{
    public partial class ObjectManager
    {
        public Dictionary<string, WorldmapCluster> GetClusters()
        {
            return ((Worldmap)(GameGui.Instance.WorldMap)).WorldmapClusters;
        }

        public WorldmapCluster GetCluster(ClusterDescriptor info)
        {
            var clusters = GetClusters();

            if (clusters.TryGetValue(info.GetIdent(), out WorldmapCluster cluster))
                return cluster;

            return default(WorldmapCluster);
        }

        public WorldmapCluster GetCluster(string name)
        {
            var clusters = GetClusters();

            foreach (var cluster in clusters.Values)
            {
                if (cluster.Info.an().ToLower() == name.ToLower())
                    return cluster;
            }

            return null;
        }
    }
}