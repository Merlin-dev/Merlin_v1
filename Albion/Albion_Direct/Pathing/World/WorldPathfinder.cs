using System;
using System.Collections.Generic;
using System.Linq;

namespace Albion_Direct.Pathing.World
{
    public class WorldPathfinder
    {
        public bool TryFindPath(ClusterDescriptor start, ClusterDescriptor end, out List<ClusterDescriptor> path)
        {
            path = new List<ClusterDescriptor> { start };
            foreach (ClusterExitDescriptor exit in start.GetExits())
            {
                if (EnumerateNode(exit, end, out List<ClusterDescriptor> newPath, 1, out int newDepth))
                {
                    path.AddRange(newPath);
                    return true;
                }
            }

            path = new List<ClusterDescriptor> { };
            return false;
        }

        private bool EnumerateNode(ClusterExitDescriptor exit, ClusterDescriptor end, out List<ClusterDescriptor> path, int depth, out int newDepth)
        {
            newDepth = depth;
            newDepth++;

            path = new List<ClusterDescriptor> { exit.GetDestination() };
            if (exit.GetDestination().GetIdent() == end.GetIdent())
            {
                return true;
            }
            else
            {
                if (newDepth > 10)
                    return false;

                if (exit.GetKind() != ClusterExitKind.Cluster)
                    return false;

                List<Tuple<int, int, List<ClusterDescriptor>>> branches = new List<Tuple<int, int, List<ClusterDescriptor>>>(4);

                foreach (ClusterExitDescriptor newExit in exit.GetDestination().GetExits())
                {
                    //Early out if neighbouring, so we don't need to sort and eval distances
                    if (newExit.GetDestination().GetIdent() == end.GetIdent())
                    {
                        newDepth++;
                        path.Add(newExit.GetDestination());
                        return true;
                    }

                    if (EnumerateNode(newExit, end, out List<ClusterDescriptor> newPath, newDepth, out int branchDepth))
                    {
                        branches.Add(new Tuple<int, int, List<ClusterDescriptor>>(branchDepth, GetScore(newExit.GetDestination()), newPath));
                    }
                }

                if (branches.Count > 0)
                {
                    if (branches.Count > 1)
                        branches = branches.OrderBy(i => i.Item1 + i.Item2).ToList();

                    path.AddRange(branches[0].Item3);
                    newDepth = branches[0].Item1;
                    return true;
                }
            }
            return false;
        }

        private int GetScore(ClusterDescriptor descriptor)
        {
            switch (descriptor.GetClusterType().GetPvpRules())
            {
                case PvpRules.PvpForced: return 255;
                case PvpRules.PvpAllowed: return 1;
            }
            return 0;
        }
    }
}