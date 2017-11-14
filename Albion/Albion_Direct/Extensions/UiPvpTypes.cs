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
        public static i7.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (i7.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this i7.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}