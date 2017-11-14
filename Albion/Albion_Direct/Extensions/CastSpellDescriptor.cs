namespace Albion_Direct
{
    public partial class CastSpellDescriptor
    {
        public string TryGetName() => !this ? "" : GetIdent();

        public SpellCategory TryGetCategory() => !this ? SpellCategory.None : Category;

        public SpellTarget TryGetTarget() => !this ? SpellTarget.None : Target;

        public float TryGetCost() => !this ? 0 : EnergyCost;
    }
}