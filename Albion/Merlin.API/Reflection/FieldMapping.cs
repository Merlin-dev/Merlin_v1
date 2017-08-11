using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class FieldMapping
    {
        /// <summary>
        /// Obfuscated field name
        /// </summary>
        public string Obfuscated;

        /// <summary>
        /// Refactored field name
        /// </summary>
        public string Refactored;
    }
}
