using System.Linq;

namespace Merlin.API.Direct
{
    public partial class LocalPlayerCharacter
    {
        public SpellSlot[] GetSpellSlotsIndexed()
        {
            SpellSlot[] slots = GetSpellSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Slot = (CharacterSpellSlot)i;
            }
            return slots;
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.sk().eg().dv().Union(_internal.sm().eg().dv()).Any(i =>
            {
                return i is arq equipableItem ? equipableItem.ai() : false;
            });
        }
    }
}