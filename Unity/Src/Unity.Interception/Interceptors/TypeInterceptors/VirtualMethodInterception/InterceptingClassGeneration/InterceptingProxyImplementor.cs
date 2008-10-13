using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This class provides the code needed to implement the <see cref="IInterceptingProxy"/>
    /// interface on a class.
    /// </summary>
    internal static class InterceptingProxyImplementor
    {
        internal static FieldBuilder ImplementIInterceptingProxy(TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(typeof (IInterceptingProxy));
            FieldBuilder pipelineManagerField = typeBuilder.DefineField("pipelineManager", typeof (PipelineManager),
                FieldAttributes.Private | FieldAttributes.InitOnly);

            ImplementGetPipeline(typeBuilder, pipelineManagerField);
            ImplementSetPipeline(typeBuilder, pipelineManagerField);

            return pipelineManagerField;
        }

        private static void ImplementGetPipeline(TypeBuilder typeBuilder, FieldInfo pipelineManagerField)
        {
            // Declaring method builder
            // Method attributes
            const MethodAttributes methodAttributes = MethodAttributes.Private | MethodAttributes.Virtual
                | MethodAttributes.Final | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot;

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod(
                    "Microsoft.Practices.Unity.InterceptionExtension.IInterceptingProxy.GetPipeline",
                    methodAttributes);

            // Setting return type
            methodBuilder.SetReturnType(typeof (HandlerPipeline));
            // Adding parameters
            methodBuilder.SetParameters(typeof (MethodBase));

            // Parameter method
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "method");
            ILGenerator il = methodBuilder.GetILGenerator();

            // Writing body
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, pipelineManagerField);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Callvirt, MethodBaseMethods.GetMetadataToken, null);
            il.Emit(OpCodes.Callvirt, PipelineManagerMethods.GetPipeline);
            il.Emit(OpCodes.Ret);
            // finished
            typeBuilder.DefineMethodOverride(methodBuilder,IInterceptingProxyMethods.GetPipeline);
        }

        private static void ImplementSetPipeline(TypeBuilder typeBuilder, FieldInfo pipelineManagerField)
        {
            // Declaring method builder
            // Method attributes
            const MethodAttributes methodAttributes = MethodAttributes.Private | MethodAttributes.Virtual
                | MethodAttributes.Final | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot;

            MethodBuilder methodBuilder =
                typeBuilder.DefineMethod(
                    "Microsoft.Practices.Unity.InterceptionExtension.IInterceptingProxy.SetPipeline",
                    methodAttributes);

            // Setting return type
            methodBuilder.SetReturnType(typeof (void));
            // Adding parameters
            methodBuilder.SetParameters(typeof (MethodBase), typeof (HandlerPipeline));
            // Parameter method
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "method");
            // Parameter pipeline
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "pipeline");

            ILGenerator il = methodBuilder.GetILGenerator();
            // Writing body
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, pipelineManagerField);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Callvirt, MethodBaseMethods.GetMetadataToken, null);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Callvirt, PipelineManagerMethods.SetPipeline);
            il.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, IInterceptingProxyMethods.SetPipeline);
        }
    }
}