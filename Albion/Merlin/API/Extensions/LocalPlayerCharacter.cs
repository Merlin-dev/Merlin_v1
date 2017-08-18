using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Direct
{
    public partial class LocalPlayerCharacter
    {
        public SpellSlot[] GetSpellSlotsIndexed()
        {
            SpellSlot[] slots = GetSpellSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].Slot = (SpellSlotIndex)i;
            }
            return slots;
        }
    }
}
