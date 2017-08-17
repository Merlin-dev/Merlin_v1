using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class HarvestableObjectExtensions
    {
        public static bool CanLoot(this HarvestableObject instance, LocalPlayerCharacterView localPlayer)
        {
            if (instance.IsHarvestable()) //Wrong name: IsProtected is valid name or something :) or IsDepleted
            {
                return false;
            }

            //instance.Internal.se().ak();

            return true;
        }
    }
}
