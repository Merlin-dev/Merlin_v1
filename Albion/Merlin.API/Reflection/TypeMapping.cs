using System;
using System.Collections.Generic;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class TypeMapping
    {
        /// <summary>
        /// Obfuscated class name
        /// </summary>
        public string Obfuscated;

        /// <summary>
        /// Refactore class name
        /// </summary>
        public string Refactored;

        private List<FieldMapping> fields = new List<FieldMapping>();
        private List<MethodMapping> methods = new List<MethodMapping>();
        private List<PropertyMapping> properties = new List<PropertyMapping>();

        public List<FieldMapping> Fields => fields;

        public List<MethodMapping> Methods => methods;

        public List<PropertyMapping> Properties => properties;
    }
}
