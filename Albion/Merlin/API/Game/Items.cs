












using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Merlin.API
{
	public class Items
	{
		#region Static

		#endregion

		#region Fields

		#endregion

		#region Properties and Events

		#endregion

		#region Constructors and Cleanup

		#endregion

		#region Methods

	
	    public static string GetItemString(lf item)
	    {
	        return item.s();
	    }

	    public static short GetItemMaxStack(lf item)
	    {
	        return item.aa();
	    }

	    public static Guid GetItemGUID(lf item)
	    {
	        return item.ac();
	    }

	    public static int GetItemInventorySlot(lf item)
	    {
	        return item.ae();
	    }

	    public static short GetItemQuantity(lf item)
	    {
	        return item.ag();
	    }

	    public static int GetItemDurability(lf item)
	    {
	        var getOtherValue = item as aro;
	        return (a4w.b(getOtherValue.b3(), getOtherValue.b5()));
	    }

	    public static int GetItemId(lf item)
	    {
	        return item.q();
	    }

	    public static int GetItemTier(lf item)
	    {
	        return item.t();
	    }

	    public static string GetItemName(lf item)
	    {
	        return item.v();
	    }

        #endregion
    }
}