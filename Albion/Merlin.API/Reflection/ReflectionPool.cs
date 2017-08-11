using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Merlin.API.Reflection
{
    public sealed class ReflectionPool
    {
        private Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        private Dictionary<string, string> typeDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> obfTypeDictionary = new Dictionary<string, string>();

        private Dictionary<string, List<FieldMapping>> fields = new Dictionary<string, List<FieldMapping>>();
        public Dictionary<string, List<MethodMapping>> methods = new Dictionary<string, List<MethodMapping>>();
        private Dictionary<string, List<PropertyMapping>> properties = new Dictionary<string, List<PropertyMapping>>();

        private Dictionary<string, Dictionary<string, MethodInfo>> methodInfoCache = new Dictionary<string, Dictionary<string, MethodInfo>>();
        private Dictionary<string, Dictionary<string, FieldInfo>> fieldInfoCache = new Dictionary<string, Dictionary<string, FieldInfo>>();
        private Dictionary<string, Dictionary<string, PropertyInfo>> propInfoCache = new Dictionary<string, Dictionary<string, PropertyInfo>>();

        public string Version = null;

        public Type FindType(string name)
        {
            if(!typeCache.ContainsKey(name) && typeDictionary.TryGetValue(name, out string obfuscated))
            {
                typeCache.Add(name, Utils.FindTypeByName(obfuscated));
            }


            if(typeCache.TryGetValue(name, out Type type))
            {
                return type;
            }

            return null;
        }

        public FieldInfo FindField(Type type, string name)
        {
            if (!obfTypeDictionary.TryGetValue(type.Name, out string refType) && !obfTypeDictionary.TryGetValue(type.FullName, out refType))
            {
                return null;
            }


            if (!fields.TryGetValue(refType, out List<FieldMapping> fieldHooks))
            {
                return null;
            }

            if(!fieldInfoCache.TryGetValue(type.FullName,out Dictionary<string, FieldInfo> cache)){
                cache = new Dictionary<string, FieldInfo>();
                fieldInfoCache.Add(type.FullName, cache);
            }
            else if(cache.TryGetValue(name, out FieldInfo fieldInfo))
            {
                return fieldInfo;
            }

            foreach (FieldMapping c in fieldHooks)
            {
                if(c.Refactored == name)
                {
                    FieldInfo info = type.GetField(c.Obfuscated, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    cache.Add(name, info);
                    return info;
                }
            }

            return null;
        }

        public FieldInfo FindField(string parent, string name) => FindField(FindType(parent), name);

        public FieldInfo FindGenericField(Type type, string name, Type[] types)
        {

            type = type.MakeGenericType(types);

            if (!obfTypeDictionary.TryGetValue(type.Name, out string refType))
            {
                return null;
            }

            if(!fields.TryGetValue(refType, out List<FieldMapping> fieldHooks))
            {
                return null;
            }

            if (!fieldInfoCache.TryGetValue(type.FullName, out Dictionary<string, FieldInfo> cache))
            {
                cache = new Dictionary<string, FieldInfo>();
                fieldInfoCache.Add(type.FullName, cache);
            }
            else if (cache.TryGetValue(name + Utils.SignatureOf(types), out FieldInfo fieldInfo))
            {
                return fieldInfo;
            }

            foreach (FieldMapping c in fieldHooks)
            {
                if (c.Refactored == name)
                {
                    FieldInfo info = type.GetField(c.Obfuscated, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    cache.Add(name + Utils.SignatureOf(types), info);
                    return info;
                }
            }

            return null;
        }

        public FieldInfo FindGenericField(string parent, string name, Type[] types) => FindGenericField(FindType(parent), name, types);

        public MethodInfo FindMethod(Type type, string name)
        {
            if (!obfTypeDictionary.TryGetValue(type.FullName, out string refType))
            {
                return null;
            }

            if (!methods.TryGetValue(refType, out List<MethodMapping> methodHooks))
            {
                return null;
            }

            if (!methodInfoCache.TryGetValue(type.FullName, out Dictionary<string, MethodInfo> cache))
            {
                cache = new Dictionary<string, MethodInfo>();
                methodInfoCache.Add(type.FullName, cache);
            }
            else if (cache.TryGetValue(name, out MethodInfo methodInfo))
            {
                return methodInfo;
            }

            foreach (MethodMapping c in methodHooks)
            {
                if (c.Refactored == name)
                {
                    if (String.IsNullOrEmpty(c.Signature))
                    {
                        MethodInfo info = type.GetMethod(c.Obfuscated, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        cache.Add(name, info);
                        return info;
                    }
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo cm = methods[i];
                        if (cm.Name == c.Obfuscated && Utils.SignatureOf(cm).Equals(c.Signature))
                        {
                            cache.Add(name, cm);
                            return cm;
                        }
                    }
                }
            }

            return null;
        }

        public MethodInfo FindMethod(string parent, string name) => FindMethod(FindType(parent), name);

        public MethodInfo FindGenericMethod(Type type, string name, Type[] types)
        {
            type = type.MakeGenericType(types);
            if(!obfTypeDictionary.TryGetValue(type.FullName, out string refType))
            {
                return null;
            }

            if(!methods.TryGetValue(refType, out List<MethodMapping> methodHooks))
            {
                return null;
            }

            if(!methodInfoCache.TryGetValue(type.FullName, out Dictionary<string, MethodInfo> cache))
            {
                cache = new Dictionary<string, MethodInfo>();
                methodInfoCache.Add(type.FullName, cache);
            }
            else if(cache.TryGetValue(name, out MethodInfo methodInfo))
            {
                return methodInfo;
            }

            foreach (MethodMapping c in methodHooks)
            {
                if(c.Refactored == name)
                {
                    if (String.IsNullOrEmpty(c.Signature))
                    {
                        MethodInfo info = type.GetMethod(c.Obfuscated, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        cache.Add(name, info);
                        return info;
                    }

                    MethodInfo[] methods = type.GetMethods();
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo cm = methods[i];
                        if(cm.Name == c.Obfuscated &&   Utils.SignatureOf(cm).Equals(c.Signature))
                        {
                            cache.Add(name, cm);
                            return cm;
                        }
                    }
                }
            }

            return null;
        }

        public MethodInfo FindGenericMethod(string parent, string name, Type[] types) => FindGenericMethod(FindType(parent), name, types);

        public PropertyInfo FindProperty(Type type, string name)
        {
            if (!obfTypeDictionary.TryGetValue(type.Name, out string refType))
            {
                return null;
            }

            if (!properties.TryGetValue(refType, out List<PropertyMapping> propertyHooks))
            {
                return null;
            }

            if (!propInfoCache.TryGetValue(type.FullName, out Dictionary<string, PropertyInfo> cache))
            {
                cache = new Dictionary<string, PropertyInfo>();
                propInfoCache.Add(type.FullName, cache);
            }
            else if (cache.TryGetValue(name, out PropertyInfo propertyInfo))
            {
                return propertyInfo;
            }

            foreach (PropertyMapping c in propertyHooks)
            {
                if(c.Refactored == name)
                {
                    PropertyInfo info = type.GetProperty(c.Obfuscated, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    cache.Add(name, info);
                    return info;
                }
            }

            return null;
        }

        public PropertyInfo FindProperty(string parent, string name) => FindProperty(FindType(parent), name);

        public bool IsSubclass(Type type, object obj) => type.IsAssignableFrom(obj.GetType());

        public bool IsSubclass(string type, object obj) => IsSubclass(FindType(type), obj);

        public static ReflectionPool FromJson(string json)
        {
            ReflectionPool pool = new ReflectionPool();
            ClientMap map = JsonReader.Deserialize<ClientMap>(json);

            pool.Version = map.Version;

            System.IO.File.WriteAllText("cluster_map.json", JsonWriter.Serialize(map));

            foreach (TypeMapping type in map.Types)
            {
                if (!pool.typeDictionary.ContainsKey(type.Refactored))
                {
                    pool.typeDictionary.Add(type.Refactored, type.Obfuscated);
                    pool.fields.Add(type.Refactored, type.Fields);
                    pool.methods.Add(type.Refactored, type.Methods);
                    pool.properties.Add(type.Refactored, type.Properties);
                }
            }

            foreach (var c in pool.typeDictionary)
            {
                if (!pool.obfTypeDictionary.ContainsKey(c.Value))
                {
                    pool.obfTypeDictionary.Add(c.Value, c.Key);
                }
            }

            return pool;
        }
    }
}
