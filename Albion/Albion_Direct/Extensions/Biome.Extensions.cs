using InternalBiome = Albion.Common.GameData.WorldInfos.Biome;

namespace Albion_Direct
{
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