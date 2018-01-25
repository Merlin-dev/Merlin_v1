using System;
using System.Reflection;
using UnityEngine;

namespace Albion_Direct
{
    public static class LocalInputHandlerExtensions
    {
        private static MethodInfo _startCastInternalTarget;
        private static MethodInfo _startCastInternalPosition;
        private static MethodInfo _doActionStaticObjectInteraction;

        static LocalInputHandlerExtensions()
        {
            var inputHandlerType = typeof(LocalInputHandler);

            _doActionStaticObjectInteraction = inputHandlerType.GetMethod("DoActionStaticObjectInteraction", BindingFlags.NonPublic | BindingFlags.Instance);
            _startCastInternalTarget = inputHandlerType.GetMethod("StartCastInternal", BindingFlags.NonPublic | BindingFlags.Instance,
                                            Type.DefaultBinder, new Type[] { typeof(byte), typeof(FightingObjectView) }, null);

            _startCastInternalPosition = inputHandlerType.GetMethod("StartCastInternal", BindingFlags.NonPublic | BindingFlags.Instance,
                                            Type.DefaultBinder, new Type[] { typeof(byte), typeof(amn) }, null);
        }

        public static void Interact(this LocalInputHandler instance, WorldObjectView target, string collider = null) => _doActionStaticObjectInteraction.Invoke(instance, new object[] { target, collider ?? string.Empty });

        public static void SetSelectedObject(this LocalInputHandler instance, SimulationObjectView view)
        {
            if (view == default(SimulationObjectView))
                instance.SetSelectedObjectId(-1);
            else
                instance.SetSelectedObjectId(view.Id);
        }

        public static void AttackSelectedObject(this LocalInputHandler instance) => instance.AttackCurrentTarget();

        public static void CastOn(this LocalInputHandler instance, CharacterSpellSlot slot, FightingObjectView target) => _startCastInternalTarget.Invoke(instance, new object[] { (byte)slot, target });

        public static void CastAt(this LocalInputHandler instance, CharacterSpellSlot slot, Vector3 target) => _startCastInternalPosition.Invoke(instance, new object[] { (byte)slot, target.c() });
    }
}