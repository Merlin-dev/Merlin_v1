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
            return _internal.to().et().d4().Union(_internal.tq().et().d4()).Any(i =>
            {
                //EquipmentItemProxy
                return i is axg equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.to().et().d4().Union(_internal.tq().et().d4()).Any(i =>
            {
                //EquipmentItemProxy
                return i is axg equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(axg item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(axg item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(axg item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.to().et().d4().Union(_internal.tq().et().d4()).Any(i =>
            {
                //MountItemProxy
                return i is axg mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}