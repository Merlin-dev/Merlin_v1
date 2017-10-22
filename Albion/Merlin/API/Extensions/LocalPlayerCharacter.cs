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
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //EquipmentItemProxy
                return i is atj equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemQualityPoor(atj item)
        {
            return item.b6() <= 40;
        }
    }
}