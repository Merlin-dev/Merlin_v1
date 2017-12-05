namespace Albion_Direct
{
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