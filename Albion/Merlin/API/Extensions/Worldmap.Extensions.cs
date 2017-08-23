using Merlin.API.Direct;
using System.Collections.Generic;

namespace Merlin
{
    public static class WorldmapExtensions
    {
        public static Dictionary<string, WorldMap.WorldmapCluster> GetClusters(this WorldMap.Worldmap map) => ((Worldmap)map).WorldmapClusters;
    }
}