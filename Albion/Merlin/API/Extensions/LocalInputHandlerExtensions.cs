using System.Reflection;

namespace Merlin
{
    public static class LocalInputHandlerExtensions
    {
        private static MethodInfo _doActionStaticObjectInteraction;

        static LocalInputHandlerExtensions()
        {
            var inputHandlerType = typeof(LocalInputHandler);

            _doActionStaticObjectInteraction = inputHandlerType.GetMethod("DoActionStaticObjectInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void Interact(this LocalInputHandler instance, WorldObjectView target) => _doActionStaticObjectInteraction.Invoke(instance, new object[] { target, string.Empty });
    }
}