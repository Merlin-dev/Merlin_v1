using System;
using System.Collections.Generic;

namespace Merlin.API.Reflection
{
    [Serializable]
    public sealed class TypeMapping
    {
        public string Refactored;
        public string Obfuscated;

        public List<MethodMapping> Methods = new List<MethodMapping>();
        public List<PropertyMapping> Properties = new List<PropertyMapping>();
        public List<FieldMapping> Fields = new List<FieldMapping>();
    }
}