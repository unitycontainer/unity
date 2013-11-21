// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Represents the implementation of a method override.
    /// </summary>
    public class MethodOverride
    {
        private static MethodInfo BuildAbstractMethodInvokedExceptionMethod =
            StaticReflection.GetMethodInfo(() => MethodOverride.BuildAbstractMethodInvokedException());

        private readonly TypeBuilder typeBuilder;
        private readonly MethodInfo methodToOverride;
        private readonly ParameterInfo[] methodParameters;
        private readonly FieldBuilder proxyInterceptionPipelineField;
        private readonly Type targetType;
        private readonly GenericParameterMapper targetTypeParameterMapper;
        private readonly int overrideCount;

        internal MethodOverride(
            TypeBuilder typeBuilder,
            FieldBuilder proxyInterceptionPipelineField,
            MethodInfo methodToOverride,
            Type targetType,
            GenericParameterMapper targetTypeParameterMapper,
            int overrideCount)
        {
            this.typeBuilder = typeBuilder;
            this.proxyInterceptionPipelineField = proxyInterceptionPipelineField;
            this.methodToOverride = methodToOverride;
            this.methodParameters = methodToOverride.GetParameters();
            this.targetType = targetType;

            // if the method is inherited and the declaring type is generic, we need to map
            // the parameters of the original declaration to the actual intercepted type type
            // E.g. consider given class Type1<T> with virtual method "T Method<U>()", the mappings in 
            // different scenarios would look like:
            // Type2<S> : Type2<S>                  => S Method<U>()
            // Type2<S> : Type2<IEnumerable<S>>     => IEnumerable<S> Method<U>()
            // Type2 : Type1<IEnumerable<string>>   => IEnumerable<string> Method<U>()
            var declaringType = methodToOverride.DeclaringType;
            this.targetTypeParameterMapper =
                declaringType.IsGenericType && declaringType != methodToOverride.ReflectedType
                    ? new GenericParameterMapper(declaringType, targetTypeParameterMapper)
                    : targetTypeParameterMapper;
            this.overrideCount = overrideCount;
        }

        internal static bool MethodCanBeIntercepted(MethodInfo method)
        {
            return method != null &&
                (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
                && method.IsVirtual
                && !method.IsFinal
                && method.DeclaringType != typeof(object);
        }

        internal MethodBuilder AddMethod()
        {
            MethodBuilder delegateMethod = CreateDelegateImplementation(methodToOverride);
            return CreateMethodOverride(delegateMethod);
        }

        private string CreateMethodName(string purpose)
        {
            return "<" + methodToOverride.Name + "_" + purpose + ">__" +
                overrideCount.ToString(CultureInfo.InvariantCulture);
        }

        private static readonly OpCode[] loadArgsOpcodes = {
            OpCodes.Ldarg_1,
            OpCodes.Ldarg_2,
            OpCodes.Ldarg_3
        };

        private static void EmitLoadArgument(ILGenerator il, int argumentNumber)
        {
            if (argumentNumber < loadArgsOpcodes.Length)
            {
                il.Emit(loadArgsOpcodes[argumentNumber]);
            }
            else
            {
                il.Emit(OpCodes.Ldarg, argumentNumber + 1);
            }
        }

        private static readonly OpCode[] loadConstOpCodes = {
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4_1,
            OpCodes.Ldc_I4_2,
            OpCodes.Ldc_I4_3,
            OpCodes.Ldc_I4_4,
            OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6,
            OpCodes.Ldc_I4_7,
            OpCodes.Ldc_I4_8,
        };

        private static void EmitLoadConstant(ILGenerator il, int i)
        {
            if (i < loadConstOpCodes.Length)
            {
                il.Emit(loadConstOpCodes[i]);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, i);
            }
        }

        private static void EmitBox(ILGenerator il, Type typeOnStack)
        {
            if (typeOnStack.IsValueType || typeOnStack.IsGenericParameter)
            {
                il.Emit(OpCodes.Box, typeOnStack);
            }
        }

        private static void EmitUnboxOrCast(ILGenerator il, Type typeOnStack)
        {
            if (typeOnStack.IsValueType || typeOnStack.IsGenericParameter)
            {
                il.Emit(OpCodes.Unbox_Any, typeOnStack);
            }
            else
            {
                il.Emit(OpCodes.Castclass, typeOnStack);
            }
        }

        private MethodBuilder CreateDelegateImplementation(MethodInfo callBaseMethod)
        {
            string methodName = CreateMethodName("DelegateImplementation");

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Private | MethodAttributes.HideBySig);
            List<LocalBuilder> outOrRefLocals = new List<LocalBuilder>();

            var paramMapper = new MethodOverrideParameterMapper(methodToOverride);
            paramMapper.SetupParameters(methodBuilder, this.targetTypeParameterMapper);

            methodBuilder.SetReturnType(typeof(IMethodReturn));
            // Adding parameters
            methodBuilder.SetParameters(typeof(IMethodInvocation), typeof(GetNextInterceptionBehaviorDelegate));
            // Parameter 
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "inputs");
            // Parameter 
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "getNext");

            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(CompilerGeneratedAttributeMethods.CompilerGeneratedAttribute, new object[0]));

            ILGenerator il = methodBuilder.GetILGenerator();

            if (!this.methodToOverride.IsAbstract)
            {
                Label done = il.DefineLabel();
                LocalBuilder ex = il.DeclareLocal(typeof(Exception));

                LocalBuilder baseReturn = null;
                LocalBuilder parameters = null;
                if (MethodHasReturnValue)
                {
                    baseReturn = il.DeclareLocal(paramMapper.GetParameterType(methodToOverride.ReturnType));
                }
                LocalBuilder retval = il.DeclareLocal(typeof(IMethodReturn));

                il.BeginExceptionBlock();
                // Call the base method
                il.Emit(OpCodes.Ldarg_0);

                if (methodParameters.Length > 0)
                {
                    parameters = il.DeclareLocal(typeof(IParameterCollection));
                    il.Emit(OpCodes.Ldarg_1);
                    il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.GetArguments, null);
                    il.Emit(OpCodes.Stloc, parameters);

                    for (int i = 0; i < methodParameters.Length; ++i)
                    {
                        il.Emit(OpCodes.Ldloc, parameters);
                        EmitLoadConstant(il, i);
                        il.EmitCall(OpCodes.Callvirt, IListMethods.GetItem, null);

                        if (methodParameters[i].ParameterType.IsByRef)
                        {
                            // For out or ref, create a local variable for it, initialize, and
                            // pass address of the local to the base class call.
                            Type referredToType = paramMapper.GetElementType(methodParameters[i].ParameterType);
                            LocalBuilder refShadow = il.DeclareLocal(referredToType);
                            outOrRefLocals.Add(refShadow);
                            EmitUnboxOrCast(il, referredToType);

                            il.Emit(OpCodes.Stloc, refShadow);
                            il.Emit(OpCodes.Ldloca, refShadow);
                        }
                        else
                        {
                            EmitUnboxOrCast(il, paramMapper.GetParameterType(methodParameters[i].ParameterType));
                        }
                    }
                }

                MethodInfo baseTarget = callBaseMethod;
                if (baseTarget.IsGenericMethod)
                {
                    baseTarget = callBaseMethod.MakeGenericMethod(paramMapper.GenericMethodParameters);
                }

                il.Emit(OpCodes.Call, baseTarget);

                if (MethodHasReturnValue)
                {
                    il.Emit(OpCodes.Stloc, baseReturn);
                }

                // Generate  the return value
                il.Emit(OpCodes.Ldarg_1);
                if (MethodHasReturnValue)
                {
                    il.Emit(OpCodes.Ldloc, baseReturn);
                    if (ReturnType.IsValueType || ReturnType.IsGenericParameter)
                    {
                        il.Emit(OpCodes.Box, paramMapper.GetReturnType());
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);
                }

                // Set up output parameters - note the collection returned to the
                // IMethodReturn object contains ALL parameter values, inputs and
                // outputs.
                EmitLoadConstant(il, methodParameters.Length);
                il.Emit(OpCodes.Newarr, typeof(object));

                if (methodParameters.Length > 0)
                {
                    LocalBuilder outputParamArray = il.DeclareLocal(typeof(object[]));
                    il.Emit(OpCodes.Stloc, outputParamArray);

                    int outputParameterNum = 0;
                    for (int paramNum = 0; paramNum < methodParameters.Length; ++paramNum)
                    {
                        il.Emit(OpCodes.Ldloc, outputParamArray);
                        EmitLoadConstant(il, paramNum);
                        if (methodParameters[paramNum].ParameterType.IsByRef)
                        {
                            il.Emit(OpCodes.Ldloc, outOrRefLocals[outputParameterNum]);
                            EmitBox(il, outOrRefLocals[outputParameterNum].LocalType);
                            ++outputParameterNum;
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc, parameters);
                            EmitLoadConstant(il, paramNum);
                            il.Emit(OpCodes.Callvirt, IListMethods.GetItem);
                        }
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                    il.Emit(OpCodes.Ldloc, outputParamArray);
                }

                il.Emit(OpCodes.Callvirt, IMethodInvocationMethods.CreateReturn);
                il.Emit(OpCodes.Stloc, retval);
                il.BeginCatchBlock(typeof(Exception));
                il.Emit(OpCodes.Stloc, ex);
                // Create an exception return
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, ex);
                il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.CreateExceptionMethodReturn, null);
                il.Emit(OpCodes.Stloc, retval);
                il.EndExceptionBlock();
                il.MarkLabel(done);
                il.Emit(OpCodes.Ldloc, retval);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                #region exception-throwing implementation

                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(OpCodes.Call, BuildAbstractMethodInvokedExceptionMethod, null);
                il.EmitCall(OpCodes.Callvirt, IMethodInvocationMethods.CreateExceptionMethodReturn, null);
                il.Emit(OpCodes.Ret);

                #endregion
            }

            return methodBuilder;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Possibly agree with this, but requires more deliberate refactoring")]
        private MethodBuilder CreateMethodOverride(MethodBuilder delegateMethod)
        {
            MethodAttributes attrs =
                methodToOverride.Attributes & ~MethodAttributes.NewSlot & ~MethodAttributes.Abstract;

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodToOverride.Name, attrs);

            var paramMapper = new MethodOverrideParameterMapper(methodToOverride);
            paramMapper.SetupParameters(methodBuilder, this.targetTypeParameterMapper);

            methodBuilder.SetReturnType(paramMapper.GetParameterType(methodToOverride.ReturnType));
            methodBuilder.SetParameters(methodParameters.Select(pi => paramMapper.GetParameterType(pi.ParameterType)).ToArray());

            int paramNum = 1;
            foreach (ParameterInfo pi in methodParameters)
            {
                methodBuilder.DefineParameter(paramNum++, pi.Attributes, pi.Name);
            }

            ILGenerator il = methodBuilder.GetILGenerator();

            LocalBuilder methodReturn = il.DeclareLocal(typeof(IMethodReturn));
            LocalBuilder ex = il.DeclareLocal(typeof(Exception));
            LocalBuilder parameterArray = il.DeclareLocal(typeof(object[]));
            LocalBuilder inputs = il.DeclareLocal(typeof(VirtualMethodInvocation));

            // Create instance of VirtualMethodInvocation
            il.Emit(OpCodes.Ldarg_0); // target object

            // If we have a generic method, we want to make sure we're using the open constructed generic method
            // so when a closed generic version of the method is invoked the actual type parameters are used
            il.Emit(
                OpCodes.Ldtoken,
                methodToOverride.IsGenericMethodDefinition
                    ? methodToOverride.MakeGenericMethod(paramMapper.GenericMethodParameters)
                    : methodToOverride);
            if (methodToOverride.DeclaringType.IsGenericType)
            {
                // if the declaring type is generic, we need to get the method from the target type
                il.Emit(OpCodes.Ldtoken, targetType);
                il.Emit(OpCodes.Call, MethodBaseMethods.GetMethodForGenericFromHandle);
            }
            else
            {
                il.Emit(OpCodes.Call, MethodBaseMethods.GetMethodFromHandle); // target method
            }

            EmitLoadConstant(il, methodParameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object)); // object[] parameters
            if (methodParameters.Length > 0)
            {
                il.Emit(OpCodes.Stloc, parameterArray);

                for (int i = 0; i < methodParameters.Length; ++i)
                {
                    il.Emit(OpCodes.Ldloc, parameterArray);
                    EmitLoadConstant(il, i);
                    EmitLoadArgument(il, i);
                    if (methodParameters[i].ParameterType.IsValueType || methodParameters[i].ParameterType.IsGenericParameter)
                    {
                        il.Emit(OpCodes.Box, paramMapper.GetParameterType(methodParameters[i].ParameterType));
                    }
                    else if (methodParameters[i].ParameterType.IsByRef)
                    {
                        Type elementType = paramMapper.GetElementType(methodParameters[i].ParameterType);
                        il.Emit(OpCodes.Ldobj, elementType);
                        if (elementType.IsValueType || elementType.IsGenericParameter)
                        {
                            il.Emit(OpCodes.Box, elementType);
                        }
                    }

                    il.Emit(OpCodes.Stelem_Ref);
                }

                il.Emit(OpCodes.Ldloc, parameterArray);
            }
            il.Emit(OpCodes.Newobj, VirtualMethodInvocationMethods.VirtualMethodInvocation);
            il.Emit(OpCodes.Stloc, inputs);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, proxyInterceptionPipelineField);
            il.Emit(OpCodes.Ldloc, inputs);

            // Put delegate reference onto the stack
            il.Emit(OpCodes.Ldarg_0);

            MethodInfo invokeTarget = delegateMethod;
            if (delegateMethod.IsGenericMethod)
            {
                invokeTarget = delegateMethod.MakeGenericMethod(paramMapper.GenericMethodParameters);
            }

            il.Emit(OpCodes.Ldftn, invokeTarget);
            il.Emit(OpCodes.Newobj, InvokeInterceptionBehaviorDelegateMethods.InvokeInterceptionBehaviorDelegate);

            // And call the pipeline
            il.Emit(OpCodes.Call, InterceptionBehaviorPipelineMethods.Invoke);

            il.Emit(OpCodes.Stloc, methodReturn);

            // Was there an exception?
            Label noException = il.DefineLabel();
            il.Emit(OpCodes.Ldloc, methodReturn);
            il.EmitCall(OpCodes.Callvirt, IMethodReturnMethods.GetException, null);
            il.Emit(OpCodes.Stloc, ex);
            il.Emit(OpCodes.Ldloc, ex);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brtrue_S, noException);
            il.Emit(OpCodes.Ldloc, ex);
            il.Emit(OpCodes.Throw);

            il.MarkLabel(noException);

            // handle return value
            if (MethodHasReturnValue)
            {
                il.Emit(OpCodes.Ldloc, methodReturn);
                il.EmitCall(OpCodes.Callvirt, IMethodReturnMethods.GetReturnValue, null);
                if (ReturnType.IsValueType || ReturnType.IsGenericParameter)
                {
                    il.Emit(OpCodes.Unbox_Any, paramMapper.GetReturnType());
                }
                else
                {
                    il.Emit(OpCodes.Castclass, paramMapper.GetReturnType());
                }
            }

            // handle byref parameters
            if (methodParameters.Length > 0)
            {
                int outArgIndex = 0;
                foreach (int parameterIndex in OutputParameterIndices)
                {
                    // Get parameter value (the address) onto the stack)
                    Type elementType = paramMapper.GetElementType(methodParameters[parameterIndex].ParameterType);
                    EmitLoadArgument(il, parameterIndex);

                    // Get result of output parameter out of the results array
                    il.Emit(OpCodes.Ldloc, methodReturn);
                    il.Emit(OpCodes.Callvirt, IMethodReturnMethods.GetOutputs);
                    EmitLoadConstant(il, outArgIndex);
                    il.Emit(OpCodes.Callvirt, IListMethods.GetItem);
                    EmitUnboxOrCast(il, elementType);

                    // And store the results
                    il.Emit(OpCodes.Stobj, elementType);
                    ++outArgIndex;
                }
            }

            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        private bool MethodHasReturnValue
        {
            get { return methodToOverride.ReturnType != typeof(void); }
        }

        private Type ReturnType
        {
            get { return methodToOverride.ReturnType; }
        }

        private IEnumerable<int> OutputParameterIndices
        {
            get
            {
                for (int i = 0; i < methodParameters.Length; ++i)
                {
                    if (methodParameters[i].ParameterType.IsByRef)
                    {
                        yield return i;
                    }
                }
            }
        }

        /// <summary>
        /// Used to throw an <see cref="NotImplementedException"/> for overrides on abstract methods.
        /// </summary>
        public static Exception BuildAbstractMethodInvokedException()
        {
            return new NotImplementedException(Resources.ExceptionAbstractMethodNotImplemented);
        }
    }
}
