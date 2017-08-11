using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class PropertyMapping
    {
        /// <summary>
        /// Obfuscated property name
        /// </summary>
        public string Obfuscated;

        /// <summary>
        /// Refactored property name
        /// </summary>
        public string Refactored;
    }
}
