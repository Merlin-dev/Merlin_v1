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
    /* Internal type: dc */
    public partial class AgentsDataFile : DataFile
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private dc _internal;
        
        #region Properties
        
        public dc AgentsDataFile_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public AgentsDataFile(dc instance) : base(instance)
        {
            _internal = instance;
        }
        
        static AgentsDataFile()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator dc(AgentsDataFile instance)
        {
            return instance._internal;
        }
        
        public static implicit operator AgentsDataFile(dc instance)
        {
            return new AgentsDataFile(instance);
        }
        
        public static implicit operator bool(AgentsDataFile instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
