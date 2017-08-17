using Merlin.API.Direct;

namespace Merlin
{
    public static class CastSpellDescriptorExtensions
    {
        public static string GetName(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? "" : descriptor.GetIdent();
        public static SpellCategory GetCategory(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? SpellCategory.None : descriptor.Category;
        public static SpellTarget GetTarget(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? SpellTarget.None : descriptor.Target;
        public static int GetCost(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? 0 : descriptor.Cost;
    }
}
