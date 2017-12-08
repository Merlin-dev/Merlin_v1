using System;

namespace Albion_Direct
{
    public partial class ClusterDescriptor : IEquatable<ClusterDescriptor>
    {
        public Biome GetBiome() => _internal.@as().Biome.ToWrapped();

        public override int GetHashCode()
        {
            return _internal.ao().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClusterDescriptor);
        }

        public bool Equals(ClusterDescriptor obj)
        {
            return obj != null && obj._internal.ao().Equals(_internal.ao());
        }
    }
}