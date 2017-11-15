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
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //EquipmentItemProxy
                return i is atn equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //EquipmentItemProxy
                return i is atn equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(atn item)
        {
            return item.b6() <= 95;
        }

        public bool IsTheItemQualityPoor(atn item)
        {
            return item.b6() <= 10;
        }

        public bool IsMountBroken(ato item)
        {
            return item.b6() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //MountItemProxy
                return i is ato mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}