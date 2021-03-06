////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.11.362.118917-prod
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
    /* Internal type: bc7 */
    public partial class GuiJournalItemProxy : GuiDurableItemProxy
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private bc7 _internal;
        
        #region Properties
        
        public bc7 GuiJournalItemProxy_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public GuiJournalItemProxy(bc7 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static GuiJournalItemProxy()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator bc7(GuiJournalItemProxy instance)
        {
            return instance._internal;
        }
        
        public static implicit operator GuiJournalItemProxy(bc7 instance)
        {
            return new GuiJournalItemProxy(instance);
        }
        
        public static implicit operator bool(GuiJournalItemProxy instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
