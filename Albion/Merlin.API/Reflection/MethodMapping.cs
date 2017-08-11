using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class MethodMapping
    {
        public string Refactored;
        public string Obfuscated;
        public string Signature;
    }
}
