using System.Collections.Generic;
using WorldMap;

namespace Albion_Direct
{
    public partial class Worldmap
    {
        public Dictionary<string, WorldmapCluster> GetClusters()
        {
            return WorldmapClusters;
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
                if (((ClusterDescriptor)cluster.Info).GetName().ToLower() == name.ToLower())
                    return cluster;
            }

            return null;
        }
    }
}