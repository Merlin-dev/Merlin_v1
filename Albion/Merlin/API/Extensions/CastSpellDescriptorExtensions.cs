using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin
{
    public static class CastSpellDescriptorExtensions
    {
        //TODO: Implement enum wrappers

        public static string GetName(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? "" : descriptor.CastSpellDescriptor_Internal.js();
        public static gz.SpellCategory GetCategory(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? gz.SpellCategory.None : descriptor.CastSpellDescriptor_Internal.d4;
        public static gz.SpellTarget GetTarget(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? gz.SpellTarget.None : descriptor.CastSpellDescriptor_Internal.d1;
        public static int GetCost(this CastSpellDescriptor descriptor) => descriptor.CastSpellDescriptor_Internal == null ? 0 : descriptor.CastSpellDescriptor_Internal.dv;
    }
}
