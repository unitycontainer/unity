using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class used to generate proxy classes for doing interception on
    /// interfaces.
    /// </summary>
    public class InterfaceInterceptorClassGenerator
    {
        private readonly Type typeToIntercept;
        private static readonly AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_InterfaceProxies"), AssemblyBuilderAccess.RunAndSave);

        private FieldBuilder pipelineManagerField;
        private FieldBuilder targetField;
        private TypeBuilder typeBuilder;

        /// <summary>
        /// Create an instance of <see cref="InterfaceInterceptorClassGenerator"/> that
        /// can construct an intercepting proxy for the given interface.
        /// </summary>
        /// <param name="typeToIntercept">Type of the interface to intercept.</param>
        public InterfaceInterceptorClassGenerator(Type typeToIntercept)
        {
            this.typeToIntercept = typeToIntercept;
            CreateTypeBuilder();
        }

        /// <summary>
        /// Create the type to proxy the requested interface
        /// </summary>
        /// <returns></returns>
        public Type CreateProxyType()
        {
            int methodNum = 0;
            foreach(MethodInfo method in typeToIntercept.GetMethods())
            {
                new InterfaceMethodOverride(typeBuilder, pipelineManagerField, targetField, method, methodNum++).AddMethod();
            }

            AddConstructor();

            Type result = typeBuilder.CreateType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_InterfaceProxies.dll");
#endif
            return result;
        }

        private void AddConstructor()
        {
            Type[] paramTypes = Seq.Collect(typeToIntercept).ToArray();

            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                paramTypes);

            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            ILGenerator il = ctorBuilder.GetILGenerator();

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, PipelineManagerMethods.Constructor);
            il.Emit(OpCodes.Stfld, pipelineManagerField);

            // Initialize the target field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, targetField);

            il.Emit(OpCodes.Ret);
        }

        private void CreateTypeBuilder()
        {
            TypeAttributes newAttributes = TypeAttributes.Public | TypeAttributes.Class;

            ModuleBuilder moduleBuilder = GetModuleBuilder();
            typeBuilder = moduleBuilder.DefineType(CreateTypeName(), newAttributes);

            //DefineGenericArguments(typeBuilder, baseClass);

            pipelineManagerField = InterceptingProxyImplementor.ImplementIInterceptingProxy(typeBuilder);
            targetField = typeBuilder.DefineField("target", typeToIntercept, FieldAttributes.Private);
            typeBuilder.AddInterfaceImplementation(typeToIntercept);
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
            return assemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }

        private string CreateTypeName()
        {
            return "DynamicModule.ns.Wrapped_" + typeToIntercept.Name + "_" + Guid.NewGuid().ToString("N");
        }

    }
}