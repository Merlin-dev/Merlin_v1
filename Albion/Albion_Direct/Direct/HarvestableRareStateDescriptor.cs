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
    /* Internal type: iy.b.a */
    public partial class HarvestableRareStateDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private iy.b.a _internal;
        
        #region Properties
        
        public iy.b.a HarvestableRareStateDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public HarvestableRareStateDescriptor(iy.b.a instance)
        {
            _internal = instance;
        }
        
        static HarvestableRareStateDescriptor()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator iy.b.a(HarvestableRareStateDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator HarvestableRareStateDescriptor(iy.b.a instance)
        {
            return new HarvestableRareStateDescriptor(instance);
        }
        
        public static implicit operator bool(HarvestableRareStateDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
