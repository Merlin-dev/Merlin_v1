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
    /* Internal type: au4 */
    public partial class ConsumableItemObject : SimpleItemObject
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private au4 _internal;
        
        #region Properties
        
        public au4 ConsumableItemObject_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        
        #endregion
        
        #region Constructor
        
        public ConsumableItemObject(au4 instance) : base(instance)
        {
            _internal = instance;
        }
        
        static ConsumableItemObject()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator au4(ConsumableItemObject instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ConsumableItemObject(au4 instance)
        {
            return new ConsumableItemObject(instance);
        }
        
        public static implicit operator bool(ConsumableItemObject instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
