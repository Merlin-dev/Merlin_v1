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
    /* Internal type: ad4 */
    public partial class CollisionManager
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private ad4 _internal;
        
        #region Properties
        
        public ad4 CollisionManager_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public byte GetCollision(Point2 A_0, float A_1) => _internal.f((ajg)A_0, (float)A_1);
        
        #endregion
        
        #region Constructor
        
        public CollisionManager(ad4 instance)
        {
            _internal = instance;
        }
        
        static CollisionManager()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ad4(CollisionManager instance)
        {
            return instance._internal;
        }
        
        public static implicit operator CollisionManager(ad4 instance)
        {
            return new CollisionManager(instance);
        }
        
        public static implicit operator bool(CollisionManager instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
