using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class WorldmapExtensions
    {
        public static Dictionary<string, WorldMap.WorldmapCluster> GetClusters(this WorldMap.Worldmap map) => ((Worldmap)map).WorldmapClusters;
    }
}
