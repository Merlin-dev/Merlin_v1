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
    /* Internal type: p0 */
    public partial struct Number
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private p0 _internal;
        
        #region Properties
        
        public p0 Number_Internal => _internal;

        #endregion

        #region Fields


        #endregion

        #region Methods

        public static Number FromDouble(System.Double A_0) => p0.e((System.Double)A_0);
        public static Number FromInt64(long A_0) => p0.f((long)A_0);
        public static Number FromRaw(long A_0) => p0.e((long)A_0);
        public long GetFractionalPart() => _internal.i();
        public System.Double GetFractions() => _internal.j();
        public long GetIntegerPart() => _internal.h();
        public System.Double ToDouble() => _internal.k();

        #endregion

        #region Constructor

        public Number(p0 instance)
        {
            _internal = instance;
        }
        
        static Number()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator p0(Number instance)
        {
            return instance._internal;
        }
        
        public static implicit operator Number(p0 instance)
        {
            return new Number(instance);
        }
        #endregion
    }
}
