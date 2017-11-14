using System;

namespace Albion_Direct
{
    public partial class ClusterDescriptor : IEquatable<ClusterDescriptor>
    {
        public Biome GetBiome() => _internal.ao().Biome.ToWrapped();

        public override int GetHashCode()
        {
            return _internal.ak().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ClusterDescriptor);
        }

        public bool Equals(ClusterDescriptor obj)
        {
            return obj != null && obj._internal.ak().Equals(_internal.ak());
        }
    }
}