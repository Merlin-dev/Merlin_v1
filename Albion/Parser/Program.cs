using JsonFx.Json;
using Merlin.API.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Preprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            //This program converts from 0xB0000000's format to my own

            Source_ClientMapping src = JsonReader.Deserialize<Source_ClientMapping>(File.ReadAllText("AO v1.0.327.94396-live.json"));

            ClientMap target = new ClientMap();
            target.Version = src.Version;

            foreach (Source_TypeMapping srcType in src.Types)
            {
                TypeMapping targetType = new TypeMapping()
                {
                    Refactored = srcType.RefactoredName,
                    Obfuscated = srcType.ObfuscatedName.Replace("/", "+").Replace("<", "[").Replace(">", "]")
                };

                foreach (Source_MethodMapping srcMethod in srcType.Methods)
                {
                    StringBuilder signature = new StringBuilder();
                    signature.Append('(');
                    for (int i = 0; i < srcMethod.Parameters.Length; i++)
                    {
                        signature.Append(srcMethod.Parameters[i].Type.Replace("/", "+").Replace("<", "[").Replace(">", "]"));
                        signature.Append(',');
                    }
                    if (srcMethod.Parameters.Length > 0)
                    {
                        signature.Length--;
                    }
                    signature.Append(')');
                    signature.Append(srcMethod.ReturnType.Replace("/", "+").Replace("<", "[").Replace(">", "]"));

                    MethodMapping targetMethod = new MethodMapping()
                    {
                        Obfuscated = srcMethod.ObfuscatedName.Replace("/", "+").Replace("<", "[").Replace(">", "]"),
                        Refactored = srcMethod.RefactoredName,
                        Signature = signature.ToString()
                    };

                    targetType.Methods.Add(targetMethod);
                }

                foreach (Source_PropertyMapping srcProperty in srcType.Properties)
                {
                    PropertyMapping targetProperty = new PropertyMapping()
                    {
                        Obfuscated = srcProperty.ObfuscatedName.Replace("/", "+").Replace("<", "[").Replace(">", "]"),
                        Refactored = srcProperty.RefactoredName
                    };
                    targetType.Properties.Add(targetProperty);
                }
                target.Types.Add(targetType);
            }

            File.WriteAllText("AO v1.0.327.94396-live.converted.json",JsonWriter.Serialize(target));

            Console.Read();
        }
    }
}
