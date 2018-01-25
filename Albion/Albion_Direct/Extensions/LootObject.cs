namespace Albion_Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.mm() && !LootObject_Internal.tc();
    }
}