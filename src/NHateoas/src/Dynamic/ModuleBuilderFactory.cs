using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHateoas.Dynamic
{
    internal static class ModuleBuilderFactory
    {
        static readonly Lazy<ModuleBuilder> Lazy = new Lazy<ModuleBuilder>(ModuleBuilder, 
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static ModuleBuilder ModuleBuilder()
        {
            var assemblyName = new AssemblyName
            {
                Name = "DynHateoas_" +  Guid.NewGuid().ToString() + ".dll",
                Version = new Version(1, 0, 0, 0)
            };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        }

        public static ModuleBuilder Instance
        {
            get { return Lazy.Value; }
        }

    }
}
