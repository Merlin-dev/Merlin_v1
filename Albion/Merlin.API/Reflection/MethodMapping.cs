using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class MethodMapping
    {
        /// <summary>
        /// Obfuscated method name
        /// </summary>
        public string Obfuscated;

        /// <summary>
        /// Refactored method name
        /// </summary>
        public string Refactored;

        /// <summary>
        /// Signature of the method
        /// </summary>
        public string Signature;
    }
}
