////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.336.100246-prod
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Albion_Direct
{
    /* Internal type: b7.SpellCategory */
    public enum SpellCategory
    {
        None = 0,
        Heal = 1,
        CrowdControl = 2,
        Damage = 3,
        Buff = 4,
        Debuff = 5,
        Instant = 6,
        Cheat = 7,
        MatchBonus = 8,
        CrimeDebuff = 9,
        CrimeProtectionBuff = 10,
        FocusFire = 11,
        HealingSickness = 12,
        DiminishingReturn = 13,
        MountBuff = 14,
        FurnitureBuff = 15,
        FoodBuff = 16,
        MovementBuff = 17,
        TerritoryBuff = 18,
        ForcedMovement = 19,
        HellBuff = 20,
        Buff_Damageshield = 21,
        PortalBuff = 22
    }
    
    public static class SpellCategoryExtensions
    {
        public static b7.SpellCategory ToInternal(this SpellCategory instance)
        {
            return (b7.SpellCategory)instance;
        }
        
        public static SpellCategory ToWrapped(this b7.SpellCategory instance)
        {
            return (SpellCategory)instance;
        }
    }
}
