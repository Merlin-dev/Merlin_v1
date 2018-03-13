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
            return _internal.tp().et().d4().Union(_internal.tr().et().d4()).Any(i =>
            {
                //EquipmentItemProxy
                return i is aw5 equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.tp().et().d4().Union(_internal.tr().et().d4()).Any(i =>
            {
                //EquipmentItemProxy
                return i is aw5 equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(aw5 item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(aw5 item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(aw5 item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.tp().et().d4().Union(_internal.tr().et().d4()).Any(i =>
            {
                //MountItemProxy
                return i is aw5 mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}