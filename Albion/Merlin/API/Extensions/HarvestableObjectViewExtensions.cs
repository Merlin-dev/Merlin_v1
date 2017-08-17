using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Merlin
{
    public static class HarvestableObjectViewExtensions
    {
        public static HarvestableObject GetHarvestableObject(this HarvestableObjectView view) => view.HarvestableObject;

        public static bool CanLoot(this HarvestableObjectView view, LocalPlayerCharacterView localPlayer) => view.GetHarvestableObject().CanLoot(localPlayer);
        public static bool RequiresTool(this HarvestableObjectView view) => view.GetHarvestableObject().RequiresTool();
    }
}
