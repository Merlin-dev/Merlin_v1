using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class ClientMap
    {
        public string Version;
        public List<TypeMapping> Types = new List<TypeMapping>();
    }
}