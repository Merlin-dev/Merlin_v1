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
    /* Internal type: amn */
    public partial struct Point2
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        
        private amn _internal;
        
        #region Properties
        
        public amn Point2_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public float GetX() => _internal.g();
        public float GetY() => _internal.h();
        public float[] ToArray() => _internal.i();
        
        #endregion
        
        #region Constructor
        
        public Point2(amn instance)
        {
            _internal = instance;
        }
        
        static Point2()
        {
            
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator amn(Point2 instance)
        {
            return instance._internal;
        }
        
        public static implicit operator Point2(amn instance)
        {
            return new Point2(instance);
        }
        #endregion
    }
}
