using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using WorldMap;
using YinYang.CodeProject.Projects.SimplePathfinding.Helpers;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders;

namespace Merlin.Pathing.Worldmap
{
    public class WorldmapPathfinder : BasePathfinder<WorldmapNode, WorldmapMap, WorldmapCluster>
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
        protected override IEnumerable<WorldmapCluster> OnEnumerateNeighbors(WorldmapNode currentNode, StopFunction<WorldmapCluster> stopFunction)
        {
            List<WorldmapCluster> result = new List<WorldmapCluster>();

            var currentCluster = (ClusterDescriptor)currentNode.Value.Info;
            var currentClusterExits = currentCluster.GetExits();

            foreach (var exit in currentClusterExits)
            {
                if (exit.GetKind() != ClusterExitKind.Cluster)
                    continue;

                WorldmapCluster cluster = GameGui.Instance.WorldMap.GetCluster(exit.GetDestination().Internal.ak());
                if (cluster != null)
                    result.Add(cluster);
            }

            return result;
        }

        protected Int32 GetScore(WorldmapCluster start, WorldmapCluster end)
        {
            var cluster = (ClusterDescriptor)end.Info;

            switch (cluster.GetClusterType().GetPvpRules())
            {
                case PvpRules.PvpForced: return Int32.MaxValue;
                case PvpRules.PvpAllowed: return 1;
            }

            return 1;
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchPathfinder{TNode,TMap}.OnPerformAlgorithm"/> for more details.
        /// </summary>
        protected override void OnPerformAlgorithm(WorldmapNode currentNode, WorldmapNode neighborNode, WorldmapCluster neighborPosition, WorldmapCluster endPosition, StopFunction<WorldmapCluster> stopFunction)
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