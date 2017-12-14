namespace Albion_Direct
{
    public static class CastSpellDescriptorExtensions
    {
        public static string TryGetName(this CastSpellDescriptor spell) => !spell ? "" : spell.GetIdent();
        public static SpellCategory TryGetCategory(this CastSpellDescriptor spell) => !spell ? SpellCategory.None : spell.Category;
        public static SpellTarget TryGetTarget(this CastSpellDescriptor spell) => !spell ? SpellTarget.None : spell.Target;
        public static float TryGetCost(this CastSpellDescriptor spell) => !spell ? 0 : spell.EnergyCost;
    }
}