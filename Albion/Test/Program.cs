using Merlin.API.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ReflectionPool pool = new ReflectionPool();

            //Testing code bellow
            TypeMapping test = new TypeMapping();
            test.Obfuscated = "a6l";
            test.Refactored = "LandscapeManager";
            test.Methods.Add(new MethodMapping() { Refactored = "GetTerrainHeight", Obfuscated = "d", Signature = "(ajg,UnityEngine.RaycastHit&)System.Single" });

            //Add to pool
            pool.Temporary_AddType(test);

            //next line is here just because i need to load assemblies
            a6l aaa = null;


            var test2 = pool.FindMethod("LandscapeManager", "GetTerrainHeight");

            Console.Read();
        }
    }
}
