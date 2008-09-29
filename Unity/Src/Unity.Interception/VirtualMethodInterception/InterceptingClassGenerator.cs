using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Class that handles generating the dynamic types used for interception.
    /// </summary>
    public class InterceptingClassGenerator
    {
        private readonly Type typeToIntercept;
        private FieldBuilder pipelineField;
        private AssemblyBuilder assemblyBuilder;
        private TypeBuilder typeBuilder;

        /// <summary>
        /// Create a new <see cref="InterceptingClassGenerator"/> that will
        /// generate a wrapper class for the requested <paramref name="typeToIntercept"/>.
        /// </summary>
        /// <param name="typeToIntercept">Type to generate the wrapper for.</param>
        public InterceptingClassGenerator(Type typeToIntercept)
        {
            this.typeToIntercept = typeToIntercept;
            CreateTypeBuilder();
        }

        /// <summary>
        /// Create the wrapper class for the given type.
        /// </summary>
        /// <returns>Wrapper type.</returns>
        public Type GenerateType()
        {
            foreach (MethodInfo method in typeToIntercept.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (MethodOverride.MethodCanBeIntercepted(method))
                {
                    new MethodOverride(typeBuilder, method).AddMethod();
                }
            }

            foreach (ConstructorInfo ctor in typeToIntercept.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                AddConstructor(ctor);
            } 
            
            Type result = typeBuilder.CreateType();
            assemblyBuilder.Save("DynamicAssembly.dll");
            return result;
        }

        private void AddConstructor(ConstructorInfo ctor)
        {
            Type[] paramTypes = Sequence.ToArray(
                Sequence.Map<ParameterInfo, Type>(ctor.GetParameters(),
                    delegate(ParameterInfo item) { return item.ParameterType; }));


            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                ctor.Attributes, ctor.CallingConvention, paramTypes);

            ILGenerator il = ctorBuilder.GetILGenerator();

            ConstructorInfo pipelineManagerCtor = typeof (PipelineManager).GetConstructor(new Type[0]);

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, pipelineManagerCtor);
            il.Emit(OpCodes.Stfld, pipelineField);

            // call base class construtor
            il.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < paramTypes.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }

            il.Emit(OpCodes.Call, ctor);

            il.Emit(OpCodes.Ret);
        }

        private void ImplementIHandlerPipelineManager()
        {
            pipelineField = typeBuilder.DefineField("pipeline", typeof (PipelineManager),
                FieldAttributes.Private | FieldAttributes.InitOnly);

            ImplementGetPipelineManager();
            ImplementSetPipelineManager();
        }

        private void ImplementGetPipelineManager()
        {
            // Declaring method builder
            // Method attributes
            MethodAttributes methodAttributes =
                MethodAttributes.Private | MethodAttributes.Virtual
                    | MethodAttributes.Final | MethodAttributes.HideBySig
                        | MethodAttributes.NewSlot;

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod(
                    "Microsoft.Practices.Unity.InterceptionExtension.IHandlerPipelineManager.GetPipeline",
                    methodAttributes);

            // Preparing Reflection instances
            MethodInfo pmGetPipelineMethod = typeof (PipelineManager).GetMethod(
                "GetPipeline", BindingFlags.Instance | BindingFlags.Public,
                null, Sequence.Collect(typeof (int)), null);

            // Setting return type
            methodBuilder.SetReturnType(typeof (HandlerPipeline));
            // Adding parameters
            methodBuilder.SetParameters(typeof (int));

            // Parameter method
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "method");
            ILGenerator gen = methodBuilder.GetILGenerator();

            // Writing body
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, pipelineField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, pmGetPipelineMethod);
            gen.Emit(OpCodes.Ret);
            // finished
            typeBuilder.DefineMethodOverride(methodBuilder, typeof (IHandlerPipelineManager).GetMethod("GetPipeline"));
        }

        private void ImplementSetPipelineManager()
        {
            // Declaring method builder
            // Method attributes
            MethodAttributes methodAttributes =
                MethodAttributes.Private | MethodAttributes.Virtual
                    | MethodAttributes.Final | MethodAttributes.HideBySig
                        | MethodAttributes.NewSlot;

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod(
                    "Microsoft.Practices.Unity.InterceptionExtension.IHandlerPipelineManager.SetPipeline",
                    methodAttributes);

            // Preparing Reflection instances
            MethodInfo pmSetPipelineMethod = typeof (PipelineManager).GetMethod(
                "SetPipeline", BindingFlags.Instance | BindingFlags.Public,
                null, new Type[] {typeof (int), typeof (HandlerPipeline)},
                null);

            // Setting return type
            methodBuilder.SetReturnType(typeof (void));
            // Adding parameters
            methodBuilder.SetParameters(typeof (int), typeof (HandlerPipeline));
            // Parameter method
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "method");
            // Parameter pipeline
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "pipeline");

            ILGenerator gen = methodBuilder.GetILGenerator();
            // Writing body
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, pipelineField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Callvirt, pmSetPipelineMethod);
            gen.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, typeof (IHandlerPipelineManager).GetMethod("SetPipeline"));
        }

        private void CreateTypeBuilder()
        {
            ModuleBuilder moduleBuilder = GetModuleBuilder();
            typeBuilder = moduleBuilder.DefineType(
                "DynamicModule.ns.Wrapped_" + typeToIntercept.Name,
                typeToIntercept.Attributes,
                typeToIntercept);

            typeBuilder.AddInterfaceImplementation(typeof (IHandlerPipelineManager));

            ImplementIHandlerPipelineManager();
        }

        private ModuleBuilder GetModuleBuilder()
        {
            assemblyBuilder = GetAssemblyBuilder();
            return
                assemblyBuilder.DefineDynamicModule
                    ("DynamicModule", "DynamicModule.dll");
        }

        private static AssemblyBuilder GetAssemblyBuilder()
        {
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            return AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        }
    }
}