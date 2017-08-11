using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Preprocessor
{
    [Serializable]
    public class Source_ClientMapping
    {
        public string Version { get; set; }

        public Source_TypeMapping[] Types { get; set; }
    }

    [Serializable]
    public class Source_TypeMapping
    {
        public string RefactoredName { get; set; }
        public string ObfuscatedName { get; set; }

        public Source_MethodMapping[] Methods { get; set; }
        public Source_PropertyMapping[] Properties { get; set; }
    }

    [Serializable]
    public class Source_PropertyMapping
    {
        public string RefactoredName { get; set; }
        public string ObfuscatedName { get; set; }
        public string Type { get; set; }
        public bool HasPublicGetter { get; set; }
        public bool HasPublicSetter { get; set; }
    }

    [Serializable]
    public class Source_MethodMapping
    {
        public string RefactoredName { get; set; }
        public string ObfuscatedName { get; set; }
        public string ReturnType { get; set; }
        public bool IsStatic { get; set; }
        public bool IsPublic { get; set; }
        public Source_ParameterMapping[] Parameters { get; set; }
    }

    [Serializable]
    public class Source_ParameterMapping
    {
        public string Type { get; set; }
        public string ObfuscatedName { get; set; }
    }
}
