using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Merlin
{
	public static class HarvestableObjectViewExtensions
	{
		static HarvestableObjectViewExtensions()
		{

		}

		public static int GetTier(this HarvestableObjectView instance)
		{
			return instance.HarvestableObject.sk();
		}

		public static string GetResourceType(this HarvestableObjectView instance)
		{
			return instance.HarvestableObject.st().u;
		}

		public static int GetRareState(this HarvestableObjectView instance)
		{
			return instance.HarvestableObject.sn();
		}

		public static int GetCurrentCharges(this HarvestableObjectView instance)
		{
			return (int)instance.HarvestableObject.sj();
		}

		public static long GetMaxCharges(this HarvestableObjectView instance)
		{
			return instance.HarvestableObject.sp();
		}

		public static bool IsLootProtected(this HarvestableObjectView instance)
		{
			return !instance.HarvestableObject.sr();
		}

		public static bool CanLoot(this HarvestableObjectView instance, LocalPlayerCharacterView player)
		{
			if (instance.IsLootProtected())
				return false;

			var requiresTool = instance.RequiresTool();
			var tool = instance.GetTool(player);

			if (requiresTool && tool == null)
				return false;

			var toolProxy = a4x.a(tool) as a39;
			var durability = toolProxy != null ? a4x.b(tool.b3(), toolProxy.ba()) : -1;
			if (requiresTool && durability <= 10)
				return false;

			return true;
		}

		public static arq GetTool(this HarvestableObjectView instance, LocalPlayerCharacterView player)
		{
			return instance.HarvestableObject.az(player.LocalPlayerCharacter, true);
		}

		public static bool RequiresTool(this HarvestableObjectView instance)
		{
			return instance.HarvestableObject.se().ak();
		}
	}
}