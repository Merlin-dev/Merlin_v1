namespace Merlin.Profiles.Gatherer
{
    public struct GatherInformation
    {
        ResourceType _resourceType;
        Tier _tier;
        EnchantmentLevel _enchantmentLevel;

        public ResourceType ResourceType { get { return _resourceType; } }
        public Tier Tier { get { return _tier; } }
        public EnchantmentLevel EnchantmentLevel { get { return _enchantmentLevel; } }

        public GatherInformation(ResourceType resourceType, Tier tier, EnchantmentLevel enchantmentLevel)
        {
            _resourceType = resourceType;
            _tier = tier;
            _enchantmentLevel = enchantmentLevel;
        }
    }
}
