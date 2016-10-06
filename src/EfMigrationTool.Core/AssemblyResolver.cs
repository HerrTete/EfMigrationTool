using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EfMigrationTool.Core
{
    internal class AssemblyResolver : IDisposable
    {
        public string SearchPath { get; internal set; }

        public AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        internal Assembly Resolve(object sender, ResolveEventArgs args)
        {
            Assembly foundAssembly = null;

            var assemblyName = new AssemblyName(args.Name);
            var assemblyPath = Path.Combine(SearchPath, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                foundAssembly = Assembly.LoadFile(assemblyPath);
            }
            return foundAssembly;
        }
        public void LoadReferences(Assembly assembly)
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies();
            foreach (var referencedAssemblyName in referencedAssemblies)
            {
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName == referencedAssemblyName.FullName))
                {
                    Trace.Write("Load Reference " + referencedAssemblyName.Name);
                    var referencedAssembly = Assembly.Load(referencedAssemblyName);
                    LoadReferences(referencedAssembly);
                }
            }
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        }
    }
}