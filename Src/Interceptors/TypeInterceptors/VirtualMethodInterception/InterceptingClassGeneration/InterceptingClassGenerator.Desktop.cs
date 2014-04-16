// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    public partial class InterceptingClassGenerator
    {
        private static ModuleBuilder GetModuleBuilder()
        {
            string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return AssemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
            return AssemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }
    }
}
