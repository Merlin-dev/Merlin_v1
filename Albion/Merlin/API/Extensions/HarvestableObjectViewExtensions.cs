using Merlin.API.Direct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Merlin
{
    public static class HarvestableObjectViewExtensions
    {
        public static HarvestableObject GetHarvestableObject(this HarvestableObjectView view) => view.HarvestableObject;

    }
}
