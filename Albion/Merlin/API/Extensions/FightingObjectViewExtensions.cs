using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class FightingObjectViewExtensions
    {
        public static FightingObject GetFightingObject(this FightingObjectView view) => view.FightingObject;

        public static long GetTargetId(this FightingObjectView view) => view.GetFightingObject().GetTargetId();
    }
}
