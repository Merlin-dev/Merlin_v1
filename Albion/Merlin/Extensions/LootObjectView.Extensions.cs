using Albion_Direct;

namespace Merlin
{
    public static class LootObjectViewExtensions
    {
        public static LootObject GetLootObject(this LootObjectView view) => view.LootObject;

        public static bool CanLoot(this LootObjectView view) => view.GetLootObject().CanLoot();
    }
}