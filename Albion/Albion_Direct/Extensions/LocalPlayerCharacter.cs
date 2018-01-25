using System.Linq;

namespace Albion_Direct
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

        public bool HasAnyDamagedItem()
        {
            return _internal.th().eh().dw().Union(_internal.tj().eh().dw()).Any(i =>
            {
                //EquipmentItemProxy
                return i is avc equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.th().eh().dw().Union(_internal.tj().eh().dw()).Any(i =>
            {
                //EquipmentItemProxy
                return i is avc equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(avc item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(avc item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(avc item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.th().eh().dw().Union(_internal.tj().eh().dw()).Any(i =>
            {
                //MountItemProxy
                return i is avc mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}