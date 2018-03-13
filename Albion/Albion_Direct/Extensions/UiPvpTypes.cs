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
        public static jz.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (jz.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this jz.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}