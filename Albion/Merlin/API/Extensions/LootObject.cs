namespace Merlin.API.Direct
{
    public partial class LootObject
    {
        public bool CanLoot() => LootObject_Internal.mh() && !LootObject_Internal.sg();
    }
}