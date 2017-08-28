using Merlin.API.Direct;
using System;

namespace Merlin
{
    public static class MobViewExtensions
    {
        static MobViewExtensions()
        {

        }

        public static int GetTier(this MobView instance)
        {
            var rawTierString = instance.MobType().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
            return int.Parse(rawTierString.Remove(0, 1));
        }

        public static ResourceType? GetResourceType(this MobView instance)
        {
            var resourceTypeString = string.Empty;
            try
            {
                resourceTypeString = instance.MobType().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[2];
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
            return instance.Mob.sh();
        }
    }
}
