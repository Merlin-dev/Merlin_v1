﻿using System.Linq;

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
                return i is axq equipableItem ? IsTheItemDamaged(equipableItem) : false;
            });
        }

        public bool HasAnyBrokenItem()
        {
            return _internal.tp().et().d4().Union(_internal.tr().et().d4()).Any(i =>
            {
                //EquipmentItemProxy
                return i is axq equipableItem ? IsTheItemQualityPoor(equipableItem) : false;
            });
        }

        public bool IsTheItemDamaged(axq item)
        {
            return item.b8() <= 95;
        }

        public bool IsTheItemQualityPoor(axq item)
        {
            return item.b8() <= 10;
        }

        public bool IsMountBroken(axq item)
        {
            return item.b8() <= 0;
        }
        public bool IsMountBroken()
        {
            return _internal.tp().et().d4().Union(_internal.tr().et().d4()).Any(i =>
            {
                //MountItemProxy
                return i is axq mountableitem ? IsMountBroken(mountableitem) : false;
            });
        }
    }
}