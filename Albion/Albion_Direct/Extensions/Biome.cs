using InternalBiome = Albion.Common.GameData.WorldInfos.Biome;

namespace Albion_Direct
{
    public enum Biome
    {
        Invalid = -1,
        Steppe = 0,
        Swamp = 1,
        Highlands = 2,
        Mountains = 3,
        Forest = 4,
        Default = 5
    }

    public static class BiomeExtensions
    {
        public static InternalBiome ToInternal(this Biome instance)
        {
            return (InternalBiome)instance;
        }

        public static Biome ToWrapped(this InternalBiome instance)
        {
            return (Biome)instance;
        }
    }
}