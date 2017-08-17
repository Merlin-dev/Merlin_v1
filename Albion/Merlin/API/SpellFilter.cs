using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API
{
    public static class SpellFilter
    {
       /* public static IEnumerable<SpellSlot> Slot(this IEnumerable<SpellSlot> spells, SpellSlotIndex spellSlot)
        {
            return spells.Where<SpellSlot>(spell => spell.GetSpellDescriptor().Get == spellSlot);
        }*/

        public static IEnumerable<SpellSlot> Category(this IEnumerable<SpellSlot> spells, gz.SpellCategory category)
        {
            return spells.Where<SpellSlot>(spell => spell.GetSpellDescriptor().GetCategory() == category);
        }

        public static IEnumerable<SpellSlot> Target(this IEnumerable<SpellSlot> spells, gz.SpellTarget target)
        {
            return spells.Where<SpellSlot>(spell => spell.GetSpellDescriptor().GetTarget() == target);
        }

        public static IEnumerable<SpellSlot> Ignore(this IEnumerable<SpellSlot> spells, string name)
        {
            return spells.Where<SpellSlot>(spell => !spell.GetSpellDescriptor().GetName().Contains(name));
        }

       /* public static IEnumerable<SpellSlot> Ready(this IEnumerable<SpellSlot> spells, LocalPlayerCharacterView player)
        {
            return spells.Where<SpellSlot>(spell => player.IsCas);
        }*/
    }
}
