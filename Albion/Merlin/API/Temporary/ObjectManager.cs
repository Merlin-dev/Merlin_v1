using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WorldMap;

namespace Merlin.API.Direct
{
    public partial class ObjectManager
    {
        private static FieldInfo _getWorldmapClusters;

        public Dictionary<string, WorldmapCluster> GetClusters()
        {
            return _getWorldmapClusters.GetValue(GameGui.Instance.WorldMap) as Dictionary<string, WorldmapCluster>;
        }

        public WorldmapCluster GetCluster(ake info)
        {
            var clusters = GetClusters();

            if (clusters.TryGetValue(info.ak(), out WorldmapCluster cluster))
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
