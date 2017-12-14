using System.Collections.Generic;
using WorldMap;
using Albion_Direct;

namespace Merlin
{
    public static class WorldmapExtensions
    {
        public static Dictionary<string, WorldmapCluster> GetClusters(this WorldMap.Worldmap map) => ((Albion_Direct.Worldmap)map).WorldmapClusters;
        public static Dictionary<string, WorldmapCluster> GetClusters(this Albion_Direct.Worldmap obj) => obj.WorldmapClusters;

        public static WorldmapCluster GetCluster(this Albion_Direct.Worldmap obj, ClusterDescriptor info)
        {
            var clusters = obj.GetClusters();

            if (clusters.TryGetValue(info.GetIdent(), out WorldmapCluster cluster))
                return cluster;

            return default(WorldmapCluster);
        }

        public static WorldmapCluster GetCluster(this Albion_Direct.Worldmap obj, string name)
        {
            var clusters = obj.GetClusters();

            foreach (var cluster in clusters.Values)
            {
                if (((ClusterDescriptor)cluster.Info).GetName().ToLower() == name.ToLower())
                    return cluster;
            }

            return null;
        }
    }
}