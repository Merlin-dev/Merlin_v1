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

            bool requiresTool = RequiresTool();
            EquipmentItemProxy tool = GetTool(localPlayer);

            if(requiresTool && !tool)

            //TODO: Implement tool check

            return true;
        }

        public EquipmentItemProxy GetTool(LocalPlayerCharacterView player)
        {
            return GetTool(player.GetLocalPlayerCharacter(), true);
        }

        public bool RequiresTool()
        {
            return GetTierDescriptor().RequiresTool();
        }
    }
}