namespace Albion_Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.t3() && !LootObject_Internal.to();
    }
}