using System.Linq;

namespace Albion_Direct
{
    public partial class LocalPlayerCharacter
    {
        public SpellSlot[] GetSpellSlotsIndexed()
        {
            SpellSlot[] slots = GetSpellSlots().GetSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Slot = (CharacterSpellSlot)i;
            }
            return slots;
        }

        public bool HasAnyDamagedItem()
        {
            return _internal.ts().ev().d6().Union(_internal.tu().ev().d6()).Any(i =>
            {
                //EquipmentItemProxy
                return i is axw equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.ts().ev().d6().Union(_internal.tu().ev().d6()).Any(i =>
            {
                //EquipmentItemProxy
                return i is axw equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(axw item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(axw item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(axw item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.ts().ev().d6().Union(_internal.tu().ev().d6()).Any(i =>
            {
                //MountItemProxy
                return i is axw mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}