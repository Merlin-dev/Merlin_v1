using System;
using System.Collections.Generic;

namespace Albion_Direct
{
    public static class MobViewExtensions
    {
        private static Dictionary<string, ResourceType> _knownResourceTypes = new Dictionary<string, ResourceType>();

        static MobViewExtensions()
        {
        }

        public static int GetTier(this MobView instance)
        {
            return instance.Mob.ta().dh();
        }

        public static ResourceType? GetResourceType(this MobView instance)
        {
            //First we try to get a known value for the MobType
            var resourceMobType = instance.MobType();
            if (_knownResourceTypes.ContainsKey(resourceMobType))
                return _knownResourceTypes[resourceMobType];

            //Second we try to get a known value for the MobName
            var resourceMobName = instance.name;
            if (_knownResourceTypes.ContainsKey(resourceMobName))
                return _knownResourceTypes[resourceMobName];

            //Third we try to get a value for the MobType (we do that first, cause in more cases this is filled)
            var resourceTypeByMobType = GetResouceTypeFromString(resourceMobType, 2);
            if (resourceTypeByMobType.HasValue)
            {
                _knownResourceTypes.Add(resourceMobType, resourceTypeByMobType.Value);
                return resourceTypeByMobType.Value;
            }

            //Fourth we try to get a value for the MobName
            var resourceTypeByMobName = GetResouceTypeFromString(resourceMobName, 1);
            if (resourceTypeByMobName.HasValue)
            {
                _knownResourceTypes.Add(resourceMobName, resourceTypeByMobName.Value);
                return resourceTypeByMobName.Value;
            }

            //Well, no luck. Next time :)
            return null;
        }

        private static ResourceType? GetResouceTypeFromString(string mobString, int stringIndex)
        {
            var resourceTypeString = string.Empty;
            try
            {
                resourceTypeString = mobString.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[stringIndex];
                return (ResourceType)Enum.Parse(typeof(ResourceType), resourceTypeString, true);
            }
            catch
            {
                if (resourceTypeString.ToLowerInvariant().Equals("critter"))
                    switch (ObjectManager.GetInstance().GetCurrentCluster().GetBiome())
                    {
                        case (Biome.Steppe):
                            return ResourceType.Hide;

                        case (Biome.Swamp):
                            return ResourceType.Fiber;

                        case (Biome.Highlands):
                            return ResourceType.Rock;

                        case (Biome.Mountains):
                            return ResourceType.Ore;

                        case (Biome.Forest):
                            return ResourceType.Wood;
                    }

                return null;
            }
        }

        public static int GetRareState(this MobView instance)
        {
            return instance.Mob.td();
        }
    }
}