using System;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders;
using Albion_Direct;

namespace Merlin.Pathing.Worldmap
{
    public class WorldmapNode : BaseGraphSearchNode<WorldmapNode, ClusterDescriptor>, IComparable<WorldmapNode>
    {
        #region Fields

        /// <summary>
        /// Gets the actual score (distance to a finish).
        /// </summary>
        public Int32 Score { get; private set; }

        /// <summary>
        /// Gets or sets the estimated score.
        /// </summary>
        public Int32 EstimatedScore { get; set; }

        #endregion Fields

        #region Constructors and Cleanup

        public WorldmapNode(ClusterDescriptor cluster, WorldmapNode origin = null, Int32 score = 0, Int32 estimatedScore = 0) : base(cluster, origin)
        {
            Score = score;
            EstimatedScore = estimatedScore;
        }

        #endregion Constructors and Cleanup

        #region Methods

        /// <summary>
        /// Updates the parameters on the fly.
        /// </summary>
        public void Update(Int32 score, Int32 estimatedScore, WorldmapNode origin)
        {
            Score = score;
            EstimatedScore = estimatedScore;
            Origin = origin;
        }

        public override Boolean Equals(WorldmapNode other)
        {
            return Value.ClusterDescriptor_Internal.ao().Equals(other.Value.ClusterDescriptor_Internal.ao());
        }

        /// <summary>
        /// See <see cref="IComparable{T}.CompareTo"/> for more details.
        /// </summary>
        public Int32 CompareTo(WorldmapNode other)
        {
            return EstimatedScore.CompareTo(other.EstimatedScore);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Cluster = {0}, Score = {1}, Estimated score = {2}", Value.ClusterDescriptor_Internal.ao(), Score, EstimatedScore);
        }

        #endregion Methods
    }
}