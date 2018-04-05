////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.11.357.115208-prod
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
    /* Internal type: axm */
    public partial class SimpleItemObject : ItemObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private axm _internal;
        
        #region Properties
        
        public axm SimpleItemObject_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public SimpleItemObject(axm instance) : base(instance)
        {
            _internal = instance;
        }
        
        static SimpleItemObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator axm(SimpleItemObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator SimpleItemObject(axm instance)
        {
            return new SimpleItemObject(instance);
        }
        
        public static implicit operator bool(SimpleItemObject instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
