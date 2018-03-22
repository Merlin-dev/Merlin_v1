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
        public static j3.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (j3.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this j3.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}