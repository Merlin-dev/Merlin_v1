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
            return _internal.sl().eg().dv().Union(_internal.sn().eg().dv()).Any(i =>
            {
                //EquipmentItemProxy
                return i is art equipableItem ? equipableItem.ai() : false;
            });
        }
    }
}