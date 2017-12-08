namespace Albion_Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.mk() && !LootObject_Internal.tc();
    }
}