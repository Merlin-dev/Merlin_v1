using System;
using Albion_Direct;

namespace Merlin
{
    public static class HarvestableObjectExtensions
    {
        public static bool CanLoot(this HarvestableObject obj, LocalPlayerCharacterView localPlayer)
        {
            if (!obj.IsHarvestable())
            {
                return false;
            }

            bool requiresTool = obj.RequiresTool();
            EquipmentItemProxy tool = obj.GetTool(localPlayer);

            if (requiresTool && !tool)
                return false;

            //TODO: Implement converters
            GuiDurableItemProxy toolProxy = ClientTools.GetStackProxy(tool).GuiItemProxy_Internal as a9h;

            int durability = toolProxy ? ClientTools.SomeCalculationWithUnfloatyFloats(tool.GetUnfloatyFloat(), toolProxy.GetUnfloatyFloat()) : -1;

            if (requiresTool && durability <= 10)
                return false;

            return true;
        }

        public static EquipmentItemProxy GetTool(this HarvestableObject obj, LocalPlayerCharacterView player)
        {
            return obj.GetTool(player.GetLocalPlayerCharacter(), false);
        }

        public static int GetTier(this HarvestableObject obj)
        {
            return obj.GetResourceDescriptor().Tier;
        }

        public static ResourceType? GetResourceType(this HarvestableObject obj)
        {
            try
            {
                var resourceTypeString = obj.GetResourceDescriptor().ResourceType;
                if (resourceTypeString.Contains("_"))
                    resourceTypeString = resourceTypeString.Substring(0, resourceTypeString.IndexOf("_"));

                return (ResourceType)Enum.Parse(typeof(ResourceType), resourceTypeString, true);
            }
            catch
            {
                return null;
            }
        }

        public static bool RequiresTool(this HarvestableObject obj)
        {
            return obj.GetTierDescriptor().RequiresTool();
        }
    }
}