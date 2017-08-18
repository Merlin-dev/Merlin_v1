using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Direct
{
    public partial class CastSpellDescriptor
    {
        public string TryGetName() => CastSpellDescriptor_Internal == null ? "" : GetIdent();
        public SpellCategory TryGetCategory() => CastSpellDescriptor_Internal == null ? SpellCategory.None : Category;
        public SpellTarget TryGetTarget() => CastSpellDescriptor_Internal == null ? SpellTarget.None : Target;
        public int TryGetCost() => CastSpellDescriptor_Internal == null ? 0 : Cost;
    }
}
