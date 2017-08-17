using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API
{
    public static class SpellFilter
    {
        public static IEnumerable<SpellSlot> Slot(this IEnumerable<SpellSlot> spells, SpellSlotIndex spellSlot) => spells.Where(spell => (SpellSlotIndex)spells.ToList().IndexOf(spell) == spellSlot);
        public static IEnumerable<SpellSlot> Category(this IEnumerable<SpellSlot> spells, SpellCategory category) => spells.Where(spell => spell.GetSpellDescriptor().GetCategory() == category);
        public static IEnumerable<SpellSlot> Target(this IEnumerable<SpellSlot> spells, SpellTarget target) => spells.Where(spell => spell.GetSpellDescriptor().GetTarget() == target);
        public static IEnumerable<SpellSlot> Ignore(this IEnumerable<SpellSlot> spells, string name) => spells.Where(spell => !spell.GetSpellDescriptor().GetName().Contains(name));
        //NOTE: Ugly hack with the index :)
        public static IEnumerable<SpellSlot> Ready(this IEnumerable<SpellSlot> spells, LocalPlayerCharacterView player) => spells.Where(spell => player.GetFightingObject().IsReadyToCast((SpellSlotIndex)spells.ToList().IndexOf(spell)));
    }
}
