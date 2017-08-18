namespace Merlin.API.Direct
{
    public partial class HarvestableObject
    {
        public bool CanLoot(LocalPlayerCharacterView localPlayer)
        {
            if (IsHarvestable()) //Returns true, when monster loot is locked
            {
                return false;
            }

            //TODO: Implement tool check

            return true;
        }

        public bool RequiresTool()
        {
            return GetTierDescriptor().HarvestableTierDescriptor_Internal.ak(); //TODO: Implement in API
        }
    }
}