namespace Merlin.API.Direct
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
        public static iz.UiPvpTypes ToInternal(this UiPvpTypes instance)
        {
            return (iz.UiPvpTypes)instance;
        }

        public static UiPvpTypes ToWrapped(this iz.UiPvpTypes instance)
        {
            return (UiPvpTypes)instance;
        }
    }
}