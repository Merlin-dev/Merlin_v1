using System;
using System.Collections.Generic;
using YinYang.CodeProject.Projects.SimplePathfinding.Helpers;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders;

namespace Albion_Direct.Pathing.Worldmap
{
    public class WorldmapPathfinder : BasePathfinder<WorldmapNode, WorldmapMap, ClusterDescriptor>
    {
        #region Fields

        private ObjectManager _world;

        #endregion Fields

        #region Constructors and Cleanup

        public WorldmapPathfinder()
        {
            _world = ObjectManager.GetInstance();
        }

        #endregion Constructors and Cleanup

        #region Methods

        /// <summary>
        /// See <see cref="BaseGraphSearchPathfinder{TNode,TMap}.OnEnumerateNeighbors"/> for more details.
        /// </summary>
        protected override IEnumerable<ClusterDescriptor> OnEnumerateNeighbors(WorldmapNode currentNode, StopFunction<ClusterDescriptor> stopFunction)
        {
            List<ClusterDescriptor> result = new List<ClusterDescriptor>();

            var currentCluster = (ClusterDescriptor)currentNode.Value;
            var currentClusterExits = currentCluster.GetExits();

            foreach (var exit in currentClusterExits)
            {
                if (exit.GetKind() != ClusterExitKind.Cluster)
                    continue;

                ClusterDescriptor cluster = exit.GetDestination();
                if (cluster != null)
                    result.Add(cluster);
            }

            return result;
        }

        protected Int32 GetScore(ClusterDescriptor start, ClusterDescriptor end)
        {
            var cluster = (ClusterDescriptor)end;

            switch (cluster.GetClusterType().GetPvpRules())
            {
                case PvpRules.PvpForced: return 255;
                case PvpRules.PvpAllowed: return 1;
            }

            return 1;
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchPathfinder{TNode,TMap}.OnPerformAlgorithm"/> for more details.
        /// </summary>
        protected override void OnPerformAlgorithm(WorldmapNode currentNode, WorldmapNode neighborNode, ClusterDescriptor neighborPosition, ClusterDescriptor endPosition, StopFunction<ClusterDescriptor> stopFunction)
        {
            Int32 neighborScore = currentNode.Score + GetScore(currentNode.Value, neighborPosition);

            // opens node at this position
            if (neighborNode == null)
            {
                Map.OpenNode(neighborPosition, currentNode, neighborScore, neighborScore);
            }
            else if (neighborScore < neighborNode.Score)
            {
                neighborNode.Update(neighborScore, neighborScore, currentNode);
            }
        }

        #endregion Methods
    }
}