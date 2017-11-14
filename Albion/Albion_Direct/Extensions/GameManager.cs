using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Albion_Direct
{
    public partial class GameManager
    {
        public float Zoom
        {
            get => (GetLocalPlayerCharacterView()?.GetComponent<LocalActorCameraController>()?.Outside.Far.Distance).GetValueOrDefault();
            set
            {
                var player = GetLocalPlayerCharacterView();
                if (player != null)
                {
                    var camera = player.GetComponent<LocalActorCameraController>();
                    if (camera != null)
                        camera.Outside.Far.Distance = value;
                }
            }
        }

        public bool GlobalFog
        {
            get
            {
                GlobalFog component = Camera.main.GetComponent<GlobalFog>();
                return component != null && (bool)component.GetType().InvokeMember("a", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, Type.DefaultBinder, component, null);
            }
            set
            {
                GlobalFog component = Camera.main.GetComponent<GlobalFog>();
                if (component != null)
                {
                    component.GetType().InvokeMember("a", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, Type.DefaultBinder, component, new object[] { value });
                    component.GetType().InvokeMember("b", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField, Type.DefaultBinder, component, new object[] { value });
                }
            }
        }

        public List<T> GetEntities<T>(Func<T, bool> selector) where T : SimulationObjectView
        {
            var list = new List<T>();

            foreach (var entity in ObjectManager.GetInstance().GetObjectMap().Values)
            {
                if (GetView(entity) is T t && selector(t))
                    list.Add(t);
            }

            return list;
        }

        public SimulationObjectView GetView_Safe(long id)
        {
            if (id > 0L)
                return GetView(id);

            return default(SimulationObjectView);
        }
    }
}