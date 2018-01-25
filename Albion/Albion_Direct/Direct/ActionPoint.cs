﻿////////////////////////////////////////////////////////////////////////////////////
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
    public partial struct ActionPoint
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();

        private ag3 _internal;

        #region Properties

        public ag3 ActionPoint_Internal => _internal;

        #endregion

        #region Fields

        public Point2 Position
        {
            get => _internal.a;
            set => _internal.a = value;
        }
        public float Radius
        {
            get => _internal.b;
            set => _internal.b = value;
        }

        #endregion

        #region Methods


        #endregion

        #region Constructor

        public ActionPoint(ag3 instance)
        {
            _internal = instance;
        }

        static ActionPoint()
        {

        }

        #endregion

        #region Conversion

        public static implicit operator ag3(ActionPoint instance)
        {
            return instance._internal;
        }

        public static implicit operator ActionPoint(ag3 instance)
        {
            return new ActionPoint(instance);
        }
        #endregion
    }
}
