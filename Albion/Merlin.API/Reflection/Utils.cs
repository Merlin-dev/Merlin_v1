using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Merlin.API.Reflection
{
    public static class Utils
    {

        public static Type FindTypeByName(string name)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type type = FindTypeByName(assemblies[i], name);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            catch (TypeLoadException)
            {
            }
            return null;
        }

        public static Type FindTypeByName(Assembly assembly, string name)
        {
            try
            {
                Module[] modules = assembly.GetModules();
                for (int i = 0; i < modules.Length; i++)
                {
                    Type type = FindTypeByName(modules[i], name);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            catch (TypeLoadException)
            {
            }
            return null;
        }

        public static Type FindTypeByName(Module module, string name)
        {
            try
            {
                Type[] types = module.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = FindTypeByName(types[i], name);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            catch (TypeLoadException)
            {
            }
            return null;
        }

        public static Type FindTypeByName(Type type, string name)
        {
            if (type.FullName.Equals(name))
            {
                return type;
            }
            try
            {
                Type[] nestedTypes = type.GetNestedTypes();
                for (int i = 0; i < nestedTypes.Length; i++)
                {
                    Type result = FindTypeByName(nestedTypes[i], name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("GetNestedTypes not supported by " + type.Name);
            }
            return null;
        }

        public static string SignatureOf(MethodInfo md)
        {
            StringBuilder signature = new StringBuilder();
            signature.Append('(');
            ParameterInfo[] parameters = md.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                signature.Append(parameters[i].ParameterType.ToString());
                signature.Append(',');
            }
            if (md.GetParameters().Length > 0)
            {
                signature.Length--;
            }
            signature.Append(')');
            signature.Append(md.ReturnType.ToString());
            return signature.ToString();
        }

        public static string SignatureOf(MethodInfo md, Type[] generics)
        {
            string signature = SignatureOf(md);
            signature += "<";
            for (int i = 0; i < generics.Length; i++)
            {
                signature += generics[i].FullName;
                signature += ",";
            }
            if (generics.Length > 0)
            {
                signature = signature.Substring(0, signature.Length - 1);
            }
            return signature + ">";
        }

        public static string SignatureOf(FieldInfo fd, Type[] generics)
        {
            string signature = fd.Name;
            signature += "<";
            for (int i = 0; i < generics.Length; i++)
            {
                signature += generics[i].FullName;
                signature += ",";
            }
            if (generics.Length > 0)
            {
                signature = signature.Substring(0, signature.Length - 1);
            }
            return signature + ">";
        }

        public static string SignatureOf(Type[] generics)
        {
            string signature = string.Empty;
            signature += "<";
            for (int i = 0; i < generics.Length; i++)
            {
                signature += generics[i].FullName;
                signature += ",";
            }
            if (generics.Length > 0)
            {
                signature = signature.Substring(0, signature.Length - 1);
            }
            return signature + ">";
        }
    }
}
