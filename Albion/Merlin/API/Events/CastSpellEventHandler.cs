












using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Merlin.API
{
	public class CastSpellEventHandler
	{
		#region Static

		#endregion

		#region Fields

		private au3 _internalHandler;

		#endregion

		#region Properties and Events

		#endregion

		#region Constructors and Cleanup
		
		public CastSpellEventHandler(au3 internalHandler)
		{
			_internalHandler = internalHandler;
		}

		#endregion

		#region Methods

		public bool IsReady(byte index)
		{
            return _internalHandler.f(index);
		}

		#endregion
	}
}