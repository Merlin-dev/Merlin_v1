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
                return i is atl equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //EquipmentItemProxy
                return i is atl equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(atl item)
        {
            return item.b6() <= 95;
        }

        public bool IsTheItemQualityPoor(atl item)
        {
            return item.b6() <= 10;
        }

        public bool IsMountBroken(atm item)
        {
            return item.b6() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.s7().eg().dv().Union(_internal.s9().eg().dv()).Any(i =>
            {
                //MountItemProxy
                return i is atm mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}