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
    /* Internal type: arw */
    public partial class FarmableItemProxy : SimpleItemProxy
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private arw _internal;
        
        #region Properties
        
        public arw FarmableItemProxy_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public FarmableItemProxy(arw instance) : base(instance)
        {
            _internal = instance;
        }
        
        static FarmableItemProxy()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator arw(FarmableItemProxy instance)
        {
            return instance._internal;
        }
        
        public static implicit operator FarmableItemProxy(arw instance)
        {
            return new FarmableItemProxy(instance);
        }
        
        public static implicit operator bool(FarmableItemProxy instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
