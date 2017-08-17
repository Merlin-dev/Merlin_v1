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
            if (instance.IsHarvestable()) //Returns true, when monster loot is locked
            {
                return false;
            }

            //TODO: Implement tool check

            return true;
        }

        public static bool RequiresTool(this HarvestableObject instance)
        {
            return instance.GetTierDescriptor().HarvestableTierDescriptor_Internal.ak(); //TODO: Implement in API
        }
    }
}
