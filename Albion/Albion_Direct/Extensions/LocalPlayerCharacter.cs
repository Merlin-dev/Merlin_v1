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
                return i is ax5 equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.ts().ev().d6().Union(_internal.tu().ev().d6()).Any(i =>
            {
                //EquipmentItemProxy
                return i is ax5 equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(ax5 item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(ax5 item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(ax5 item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.ts().ev().d6().Union(_internal.tu().ev().d6()).Any(i =>
            {
                //MountItemProxy
                return i is ax5 mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}