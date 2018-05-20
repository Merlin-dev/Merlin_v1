namespace Albion_Direct
{
    public enum UiPvpTypes
    {
        Safe = 0,
        Limited = 1,
        Full = 2,
        Black = 3
    }

    public static class UiPvpTypesExtensions
    {
        public static kc.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (kc.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this kc.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}