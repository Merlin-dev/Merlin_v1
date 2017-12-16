////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.0.336.100246-prod
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

namespace Albion_Direct
{
    /* Internal type: au9 */
    public partial class DurableItemObject : ItemObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private au9 _internal;
        
        #region Properties
        
        public au9 DurableItemObject_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public DurableItemObject(au9 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static DurableItemObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator au9(DurableItemObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator DurableItemObject(au9 instance)
        {
            return new DurableItemObject(instance);
        }
        
        public static implicit operator bool(DurableItemObject instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
