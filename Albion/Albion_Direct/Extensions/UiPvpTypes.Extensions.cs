namespace Albion_Direct
{
    public static class UiPvpTypesExtensions
    {
        public static jl.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (jl.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this jl.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}