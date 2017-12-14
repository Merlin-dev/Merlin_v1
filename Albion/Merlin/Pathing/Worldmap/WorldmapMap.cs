﻿using System;
using YinYang.CodeProject.Projects.SimplePathfinding.Helpers;
using YinYang.CodeProject.Projects.SimplePathfinding.PathFinders;
using Albion_Direct;

namespace Merlin.Pathing.Worldmap
{
    public class WorldmapMap : BaseGraphSearchMap<WorldmapNode, ClusterDescriptor>
    {
        #region Fields

        private readonly PriorityQueue<WorldmapNode> priorityQueue;

        #endregion Fields

        #region Constructors and Cleanup

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldmapMap{TNode}"/> class.
        /// </summary>
        public WorldmapMap() : base()
        {
            priorityQueue = new PriorityQueue<WorldmapNode>();
        }

        #endregion Constructors and Cleanup

        #region Methods

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnCreateFirstNode"/> for more details.
        /// </summary>
        protected override WorldmapNode OnCreateFirstNode(ClusterDescriptor startPosition, ClusterDescriptor endPosition)
        {
            return new WorldmapNode(startPosition, null, 0, 0);
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnCreateNode"/> for more details.
        /// </summary>
        protected override WorldmapNode OnCreateNode(ClusterDescriptor position, WorldmapNode origin, params object[] arguments)
        {
            Int32 score = arguments != null && arguments.Length > 0 ? (Int32)arguments[0] : 0;
            Int32 estimatedScore = arguments != null && arguments.Length > 1 ? (Int32)arguments[1] : 0;

            return new WorldmapNode(position, origin, score, estimatedScore);
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnGetCount"/> for more details.
        /// </summary>
        protected override Int32 OnGetCount()
        {
            return priorityQueue.Count;
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnAddNewNode"/> for more details.
        /// </summary>
        protected override void OnAddNewNode(WorldmapNode result)
        {
            priorityQueue.Enqueue(result);
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnGetTopNode"/> for more details.
        /// </summary>
        protected override WorldmapNode OnGetTopNode()
        {
            return priorityQueue.Dequeue();
        }

        /// <summary>
        /// See <see cref="BaseGraphSearchMap{TNode}.OnClear"/> for more details.
        /// </summary>
        protected override void OnClear()
        {
            priorityQueue.Clear();
        }

        #endregion Methods
    }
}