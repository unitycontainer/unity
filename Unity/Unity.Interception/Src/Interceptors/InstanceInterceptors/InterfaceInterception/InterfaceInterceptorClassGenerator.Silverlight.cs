using System;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    public partial class InterfaceInterceptorClassGenerator
    {
        private static ModuleBuilder moduleBuilder;
        private static readonly object moduleBuilderLock = new object();

        private static ModuleBuilder GetModuleBuilder()
        {
            lock (moduleBuilderLock)
            {
                if (moduleBuilder == null)
                {
                    string moduleName = Guid.NewGuid().ToString("N");
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
                }
            }
            return moduleBuilder;
        }
    }
}