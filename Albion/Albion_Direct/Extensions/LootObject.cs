namespace Albion_Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.t1() && !LootObject_Internal.tl();
    }
}