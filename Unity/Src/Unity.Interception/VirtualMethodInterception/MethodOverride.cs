using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    class MethodOverride
    {
        private readonly TypeBuilder typeBuilder;
        private readonly MethodInfo methodToOverride;

        #region Various methods we need to emit calls to

        private static readonly MethodInfo getPipeline = typeof (IHandlerPipelineManager).GetMethod("GetPipeline");

        private static readonly ConstructorInfo methodInvocationCtor = typeof (VirtualMethodInvocation).GetConstructor(
            Sequence.Collect(typeof (object), typeof (MethodBase), typeof (object[])));

        private static readonly MethodInfo getMethodFromHandle = typeof (MethodBase).GetMethod("GetMethodFromHandle",
            BindingFlags.Public | BindingFlags.Static, null, Sequence.Collect(typeof (RuntimeMethodHandle)), null);

        private static readonly ConstructorInfo invokeHandlerDelegateCtor = typeof (InvokeHandlerDelegate).GetConstructor(
            Sequence.Collect(typeof (object), typeof (IntPtr)));

        private static readonly MethodInfo pipelineInvoke = typeof (HandlerPipeline).GetMethod("Invoke");

        private static readonly ConstructorInfo compilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).GetConstructor(new Type[0]);
        private static readonly MethodInfo createReturn = typeof(IMethodInvocation).GetMethod("CreateMethodReturn");
        private static readonly MethodInfo createExceptionMethodReturn = typeof (IMethodInvocation).GetMethod("CreateExceptionMethodReturn");
        private static readonly MethodInfo getArguments = typeof(IMethodInvocation).GetProperty("Arguments").GetGetMethod();

        private static readonly MethodInfo getException = typeof (IMethodReturn).GetProperty("Exception").GetGetMethod();
        private static readonly MethodInfo getReturnValue = typeof (IMethodReturn).GetProperty("ReturnValue").GetGetMethod();

        #endregion

        public MethodOverride(TypeBuilder typeBuilder, MethodInfo methodToOverride)
        {
            this.typeBuilder = typeBuilder;
            this.methodToOverride = methodToOverride;
        }

        public static bool MethodCanBeIntercepted(MethodInfo method)
        {
            return (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
                && method.IsVirtual
                && method.GetParameters().Length == 0
                && method.DeclaringType != typeof(object);
        }

        public void AddMethod()
        {
            MethodBuilder callBaseMethod = CreateBaseCallMethod();
            MethodBuilder delegateMethod = CreateDelegateImplementation(callBaseMethod);
            CreateMethodOverride(delegateMethod);
        }

        private MethodBuilder CreateBaseCallMethod()
        {
            string methodName = string.Format("<{0}_Callbase>b__0", methodToOverride.Name);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Private | MethodAttributes.HideBySig,
                methodToOverride.ReturnType, Seq.Make(methodToOverride.GetParameters())
                    .Map<Type>(delegate(ParameterInfo pi) { return pi.ParameterType; })
                    .ToArray());

            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, methodToOverride);
            il.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private MethodBuilder CreateDelegateImplementation(MethodInfo callBaseMethod)
        {
            string methodName = string.Format("<{0}_DelegateImplementation>b__0", methodToOverride.Name);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Private | MethodAttributes.HideBySig);
            methodBuilder.SetReturnType(typeof(IMethodReturn));
            // Adding parameters
            methodBuilder.SetParameters(typeof(IMethodInvocation), typeof(GetNextHandlerDelegate));
            // Parameter 
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "inputs");
            // Parameter 
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "getNext");

            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(compilerGeneratedAttribute, new object[0]));

            ILGenerator il = methodBuilder.GetILGenerator();
            Label done = il.DefineLabel();
            LocalBuilder ex = il.DeclareLocal(typeof (Exception));

            LocalBuilder baseReturn = null;

            if (MethodHasReturnValue)
            {
                baseReturn = il.DeclareLocal(methodToOverride.ReturnType);
            }
            LocalBuilder retval = il.DeclareLocal(typeof (IMethodReturn));

            il.BeginExceptionBlock();
            // Call the base method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, callBaseMethod);
            if(MethodHasReturnValue)
            {
                il.Emit(OpCodes.Stloc, baseReturn);
            }

            // Generate  the return value
            il.Emit(OpCodes.Ldarg_1);
            if(MethodHasReturnValue)
            {
                il.Emit(OpCodes.Ldloc, baseReturn);
                if(ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Box, ReturnType);
                }
            }
            else
            {
                il.Emit(OpCodes.Ldnull);
            }
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Newarr, typeof (object));
            il.Emit(OpCodes.Callvirt, createReturn);
            il.Emit(OpCodes.Stloc, retval);
            il.Emit(OpCodes.Leave, done);
            il.BeginCatchBlock(typeof (Exception));
            il.Emit(OpCodes.Stloc, ex);
            // Create an exception return
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, ex);
            il.EmitCall(OpCodes.Call, createExceptionMethodReturn, null);
            il.Emit(OpCodes.Stloc, retval);
            il.EndExceptionBlock();
            il.MarkLabel(done);
            il.Emit(OpCodes.Ldloc, retval);
            il.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private void CreateMethodOverride(MethodBuilder delegateMethod)
        {
            MethodAttributes attrs = methodToOverride.Attributes & ~MethodAttributes.NewSlot;

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodToOverride.Name, attrs,
                methodToOverride.CallingConvention, ReturnType,
                new Type[0]);

            ILGenerator il = methodBuilder.GetILGenerator();

            LocalBuilder methodReturn = il.DeclareLocal(typeof (IMethodReturn));
            LocalBuilder ex = il.DeclareLocal(typeof (Exception));

            // Get pipeline for this method onto the stack
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, typeof (IHandlerPipelineManager));
            il.Emit(OpCodes.Ldc_I4, methodToOverride.MetadataToken);
            il.EmitCall(OpCodes.Callvirt, getPipeline, null);

            // Create instance of VirtualMethodInvocation
            il.Emit(OpCodes.Ldarg_0); // target object

            il.Emit(OpCodes.Ldtoken, methodToOverride);
            il.Emit(OpCodes.Call, getMethodFromHandle); // target method

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Newarr, typeof (object)); // object[] parameters

            il.Emit(OpCodes.Newobj, methodInvocationCtor);

            // Put delegate reference onto the stack
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, delegateMethod);
            il.Emit(OpCodes.Newobj, invokeHandlerDelegateCtor);

            // And call the pipeline
            il.Emit(OpCodes.Call, pipelineInvoke);

            il.Emit(OpCodes.Stloc, methodReturn);

            // Was there an exception?
            Label noException = il.DefineLabel();
            il.Emit(OpCodes.Ldloc, methodReturn);
            il.EmitCall(OpCodes.Call, getException, null);
            il.Emit(OpCodes.Stloc, ex);
            il.Emit(OpCodes.Ldloc, ex);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brtrue_S, noException);
            il.Emit(OpCodes.Ldloc, ex);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(noException);

            if(MethodHasReturnValue)
            {
                il.Emit(OpCodes.Ldloc, methodReturn);
                il.EmitCall(OpCodes.Call, getReturnValue, null);
                if(ReturnType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, ReturnType);
                }
            }
            il.Emit(OpCodes.Ret);
        }

        private bool MethodHasReturnValue
        {
            get { return methodToOverride.ReturnType != typeof (void); }
        }

        private Type ReturnType
        {
            get { return methodToOverride.ReturnType; }
        }
    }
}
