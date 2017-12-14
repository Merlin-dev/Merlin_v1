using Albion_Direct;

namespace Merlin
{
    public static class SilverObjectViewExtensions
    {
        public static SilverObject GetSilverObject(this SilverObjectView view) => view.SilverObject;

        public static bool CanLoot(this SilverObjectView view, LocalPlayerCharacterView localPlayer) => view.GetSilverObject().CanLoot();
    }
}