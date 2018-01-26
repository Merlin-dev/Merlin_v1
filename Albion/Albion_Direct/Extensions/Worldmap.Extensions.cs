using System.Collections.Generic;

namespace Albion_Direct
{
    public static class WorldmapExtensions
    {
        public static Dictionary<string, WorldMap.WorldmapCluster> GetClusters(this WorldMap.Worldmap map) => ((Worldmap)map).WorldmapClusters;
    }
}