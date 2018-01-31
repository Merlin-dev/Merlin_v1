using System;

namespace Albion_Direct
{
    public partial class HarvestableObject
    {
        public bool CanLoot(LocalPlayerCharacterView localPlayer)
        {
            if (!IsHarvestable())
            {
                return false;
            }

            bool requiresTool = RequiresTool();
            EquipmentItemProxy tool = GetTool(localPlayer);

            if (requiresTool && !tool)
                return false;

            //TODO: Implement converters
            GuiDurableItemProxy toolProxy = ClientTools.GetStackProxy(tool).GuiItemProxy_Internal as a9m;

            int durability = toolProxy ? ClientTools.SomeCalculationWithUnfloatyFloats(tool.GetUnfloatyFloat(), toolProxy.GetUnfloatyFloat()) : -1;

            if (requiresTool && durability <= 10)
                return false;

            return true;
        }

        public EquipmentItemProxy GetTool(LocalPlayerCharacterView player)
        {
            return GetTool(player.GetLocalPlayerCharacter(), false);
        }

        public int GetTier()
        {
            return GetResourceDescriptor().Tier;
        }

        public ResourceType? GetResourceType()
        {
            try
            {
                var resourceTypeString = GetResourceDescriptor().ResourceType;
                if (resourceTypeString.Contains("_"))
                    resourceTypeString = resourceTypeString.Substring(0, resourceTypeString.IndexOf("_"));

                return (ResourceType)Enum.Parse(typeof(ResourceType), resourceTypeString, true);
            }
            catch
            {
                return null;
            }
        }

        public bool RequiresTool()
        {
            return GetTierDescriptor().RequiresTool();
        }
    }
}