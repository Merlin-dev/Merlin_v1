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
    /* Internal type: akf */
    public partial class ClusterExitDescriptor
    {
        private static List<MethodInfo> _methodReflectionPool = new List<MethodInfo>();
        private static List<PropertyInfo> _propertyReflectionPool = new List<PropertyInfo>();
        private static List<FieldInfo> _fieldReflectionPool = new List<FieldInfo>();
        
        private akf _internal;
        
        #region Properties
        
        public akf ClusterExitDescriptor_Internal => _internal;
        
        #endregion
        
        #region Fields
        
        
        #endregion
        
        #region Methods
        
        public ClusterDescriptor GetDestination() => _internal.o();
        public ClusterExitKind GetKind() => _internal.r().ToWrapped();
        public Point2 GetPosition() => _internal.v();
        public ClusterDescriptor GetSource() => _internal.l();
        public void SetDestination(ClusterDescriptor A_0) => _methodReflectionPool[0].Invoke(_internal,new object[]{(ake)A_0});
        public void SetSource(ClusterDescriptor A_0) => _methodReflectionPool[1].Invoke(_internal,new object[]{(ake)A_0});
        
        #endregion
        
        #region Constructor
        
        public ClusterExitDescriptor(akf instance)
        {
            _internal = instance;
        }
        
        static ClusterExitDescriptor()
        {
            _methodReflectionPool.Add(typeof(akf).GetMethod("l", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new Type[]{typeof(ake)}, null));
            _methodReflectionPool.Add(typeof(akf).GetMethod("m", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new Type[]{typeof(ake)}, null));
        }
        
        #endregion
        
        #region Conversion
        
        public static implicit operator akf(ClusterExitDescriptor instance)
        {
            return instance._internal;
        }
        
        public static implicit operator ClusterExitDescriptor(akf instance)
        {
            return new ClusterExitDescriptor(instance);
        }
        
        public static implicit operator bool(ClusterExitDescriptor instance)
        {
            return instance._internal != null;
        }
        #endregion
    }
}
