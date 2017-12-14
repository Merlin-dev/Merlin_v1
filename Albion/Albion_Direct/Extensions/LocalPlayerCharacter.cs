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
                return i is au7 equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.th().eh().dw().Union(_internal.tj().eh().dw()).Any(i =>
            {
                //EquipmentItemProxy
                return i is au7 equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(au7 item)
        {
            return item.b6() <= 95;
        }

        public bool IsTheItemQualityPoor(au7 item)
        {
            return item.b6() <= 10;
        }

        public bool IsMountBroken(au7 item)
        {
            return item.b6() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.th().eh().dw().Union(_internal.tj().eh().dw()).Any(i =>
            {
                //MountItemProxy
                return i is au7 mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}