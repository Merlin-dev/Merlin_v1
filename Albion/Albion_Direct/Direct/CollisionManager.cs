////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.11.362.117521-prod
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

namespace Albion_Direct
{
    /* Internal type: ai7 */
    public partial class CollisionManager
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private ai7 _internal;
        
        #region Properties
        
        public ai7 CollisionManager_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public byte GetCollision(Point2 A_0, float A_1) => _internal.f((ao6)A_0, (float)A_1);
        
        #endregion
        
        #region Constructor
        
        public CollisionManager(ai7 instance)
        {
            _internal = instance;
        }
        
        static CollisionManager()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator ai7(CollisionManager instance)
        {
            return instance._internal;
        }
        
        public static implicit operator CollisionManager(ai7 instance)
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
