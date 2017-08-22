////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.327.94396-live
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;

using Albion.Common.Time;

namespace Merlin.API.Direct
{
    /* Internal type: WorldMap.Worldmap */
    public partial class Worldmap
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private WorldMap.Worldmap _internal;
        
        #region Properties
        
        public WorldMap.Worldmap Worldmap_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        public Dictionary<string, WorldMap.WorldmapCluster> WorldmapClusters => (Dictionary<string, WorldMap.WorldmapCluster>)_fieldReflectionPool[0].GetValue(_internal);
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public Worldmap(WorldMap.Worldmap instance)
        {
            _internal = instance;
        }
        
        static Worldmap()
        {
            _fieldReflectionPool.Add(typeof(WorldMap.Worldmap).GetField("c", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator WorldMap.Worldmap(Worldmap instance)
        {
            return instance._internal;
        }
        
        public static implicit operator Worldmap(WorldMap.Worldmap instance)
        {
            return new Worldmap(instance);
        }
        
        public static implicit operator bool(Worldmap instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
