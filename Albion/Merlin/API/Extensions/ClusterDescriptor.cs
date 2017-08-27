namespace Merlin.API.Direct
{
    public partial class ClusterDescriptor
    {
        public Biome GetBiome() => _internal.ao().Biome.ToWrapped();
    }
}