using JsonFx.Json;
using Merlin.API.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
           ReflectionPool pool = ReflectionPool.FromJson(File.ReadAllText("c_map.json"));

            //This will load Albion.Common to CurrentDomain
            akf.Kind placeholder = akf.Kind.Cluster;
            //This will load Albion.PhotonClient to CurrentDomain
            alb.a();

            var test = pool.FindType("ClusterExitKind");
            var minfo = pool.FindMethod("ObjectManager", "GetCurrentCluster");
            Console.WriteLine(minfo.DeclaringType.FullName + "."+minfo.Name);
            Console.Read();
        }
    }
}
