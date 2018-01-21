////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.10.351.108124-prod
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
    /* Internal type: af1 */
    public partial class ItemsDataFile : DataFile
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private af1 _internal;
        
        #region Properties
        
        public af1 ItemsDataFile_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public ItemsDataFile(af1 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static ItemsDataFile()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator af1(ItemsDataFile instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ItemsDataFile(af1 instance)
        {
            return new ItemsDataFile(instance);
        }
        
        public static implicit operator bool(ItemsDataFile instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
