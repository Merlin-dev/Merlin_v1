////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.11.362.117031-prod
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
    /* Internal type: a76 */
    public partial class MountObject : StaticObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private a76 _internal;
        
        #region Properties
        
        public a76 MountObject_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public MountItemDescriptor GetDescriptor() => _internal.tj();
        public bool IsInRemountDistance() => _internal.tm();
        public bool IsLocalPlayers() => _internal.tk();
        
        #endregion
        
        #region Constructor
        
        public MountObject(a76 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static MountObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator a76(MountObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator MountObject(a76 instance)
        {
            return new MountObject(instance);
        }
        
        public static implicit operator bool(MountObject instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
