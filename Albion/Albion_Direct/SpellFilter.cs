using System.Collections.Generic;
using System.Linq;

namespace Albion_Direct
{
    public static class SpellFilter
    {
        public static IEnumerable<SpellSlot> Slot(this IEnumerable<SpellSlot> spells, CharacterSpellSlot spellSlot) => spells.Where(spell => spell.Slot == spellSlot);

        public static IEnumerable<SpellSlot> Category(this IEnumerable<SpellSlot> spells, SpellCategory category) => spells.Where(spell => spell.GetSpellDescriptor().TryGetCategory() == category);

        public static IEnumerable<SpellSlot> Target(this IEnumerable<SpellSlot> spells, SpellTarget target) => spells.Where(spell => spell.GetSpellDescriptor().TryGetTarget() == target);

        public static IEnumerable<SpellSlot> Ignore(this IEnumerable<SpellSlot> spells, string name) => spells.Where(spell => !spell.GetSpellDescriptor().TryGetName().Contains(name));

        public static IEnumerable<SpellSlot> Ready(this IEnumerable<SpellSlot> spells, LocalPlayerCharacterView player) => spells.Where(spell => player.GetFightingObject().IsReadyToCast(spell.Slot));
    }
}