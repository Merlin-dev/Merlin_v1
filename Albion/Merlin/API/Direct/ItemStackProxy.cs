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
    /* Internal type: a4c */
    public partial class ItemStackProxy
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private a4c _internal;
        
        #region Properties
        
        public a4c ItemStackProxy_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public ItemStackProxy(a4c instance)
        {
            _internal = instance;
        }
        
        static ItemStackProxy()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator a4c(ItemStackProxy instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ItemStackProxy(a4c instance)
        {
            return new ItemStackProxy(instance);
        }
        
        public static implicit operator bool(ItemStackProxy instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
