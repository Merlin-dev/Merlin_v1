namespace Albion_Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.t0() && !LootObject_Internal.tl();
    }
}