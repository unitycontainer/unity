//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    class MethodOverride
    {
        private readonly TypeBuilder typeBuilder;
        private readonly MethodInfo methodToOverride;
        private readonly ParameterInfo[] methodParameters;
        private readonly FieldBuilder pipelineField;
        private readonly int overrideCount;

        public MethodOverride(TypeBuilder typeBuilder, FieldBuilder pipelineField, MethodInfo methodToOverride, int overrideCount)
        {
            this.typeBuilder = typeBuilder;
            this.pipelineField = pipelineField;
            this.methodToOverride = methodToOverride;
            methodParameters = methodToOverride.GetParameters();
            this.overrideCount = overrideCount;
        }

        public static bool MethodCanBeIntercepted(MethodInfo method)
        {
            return method != null &&
                (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly)
                && method.IsVirtual
                && !method.IsFinal
                && method.DeclaringType != typeof(object);
        }

        public MethodBuilder AddMethod()
        {
            MethodBuilder callBaseMethod = CreateBaseCallMethod();
            MethodBuilder delegateMethod = CreateDelegateImplementation(callBaseMethod);
            return CreateMethodOverride(delegateMethod);
        }

        private string CreateMethodName(string purpose)
        {
            return "<" + methodToOverride.Name + "_" + purpose + ">__" +
                overrideCount.ToString(CultureInfo.InvariantCulture);
        }

        private MethodBuilder CreateBaseCallMethod()
        {
            string methodName = CreateMethodName("Callbase");
            const MethodAttributes methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, methodAttributes);

            SetupGenericParameters(methodBuilder);

            methodBuilder.SetReturnType(methodToOverride.ReturnType);
            methodBuilder.SetParameters(Seq.Make(methodParameters)
                .Map<Type>(delegate(ParameterInfo pi) { return pi.ParameterType; })
                .ToArray());
            int paramNum = 1;

            foreach (ParameterInfo pi in methodParameters)
            {
                methodBuilder.DefineParameter(paramNum++, pi.Attributes, pi.Name);
            }

            ILGenerator il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < methodParameters.Length; ++i)
            {
                EmitLoadArgument(il, i);
            }

            il.Emit(OpCodes.Call, methodToOverride);
            il.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private void SetupGenericParameters(MethodBuilder methodBuilder)
        {
            if (methodToOverride.IsGenericMethod)
            {
                Type[] genericArguments = methodToOverride.GetGenericArguments();
                string[] names = Seq.Make(genericArguments)
                    .Map<string>(delegate(Type t) { return t.Name; })
                    .ToArray();
                GenericTypeParameterBuilder[] builders = methodBuilder.DefineGenericParameters(names);
                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    builders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                    foreach (Type type in genericArguments[i].GetGenericParameterConstraints())
                    {
                        builders[i].SetBaseTypeConstraint(type);
                    }
                }
            }
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
            if(typeOnStack.IsValueType || typeOnStack.IsGenericParameter)
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

            SetupGenericParameters(methodBuilder);

            methodBuilder.SetReturnType(typeof(IMethodReturn));
            // Adding parameters
            methodBuilder.SetParameters(typeof(IMethodInvocation), typeof(GetNextHandlerDelegate));
            // Parameter 
            methodBuilder.DefineParameter(1, ParameterAttributes.None, "inputs");
            // Parameter 
            methodBuilder.DefineParameter(2, ParameterAttributes.None, "getNext");

            methodBuilder.SetCustomAttribute(new CustomAttributeBuilder(CompilerGeneratedAttributeMethods.CompilerGeneratedAttribute, new object[0]));

            ILGenerator il = methodBuilder.GetILGenerator();
            Label done = il.DefineLabel();
            LocalBuilder ex = il.DeclareLocal(typeof(Exception));

            LocalBuilder baseReturn = null;
            LocalBuilder parameters = null;
            if (MethodHasReturnValue)
            {
                baseReturn = il.DeclareLocal(methodToOverride.ReturnType);
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
                        Type referredToType = methodParameters[i].ParameterType.GetElementType();
                        LocalBuilder refShadow = il.DeclareLocal(referredToType);
                        outOrRefLocals.Add(refShadow);
                        EmitUnboxOrCast(il, referredToType);

                        il.Emit(OpCodes.Stloc, refShadow);
                        il.Emit(OpCodes.Ldloca, refShadow);
                    }
                    else
                    {
                        EmitUnboxOrCast(il, methodParameters[i].ParameterType);
                    }
                }
            }

            il.Emit(OpCodes.Call, callBaseMethod);

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
                    il.Emit(OpCodes.Box, ReturnType);
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
                LocalBuilder outputParamArray = il.DeclareLocal(typeof (object[]));
                il.Emit(OpCodes.Stloc, outputParamArray);

                int outputParameterNum = 0;
                for(int paramNum = 0; paramNum < methodParameters.Length; ++paramNum)
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
            return methodBuilder;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Possibly agree with this, but requires more deliberate refactoring")]
        private MethodBuilder CreateMethodOverride(MethodBuilder delegateMethod)
        {
            MethodAttributes attrs = methodToOverride.Attributes & ~MethodAttributes.NewSlot;

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodToOverride.Name, attrs);

            SetupGenericParameters(methodBuilder);

            methodBuilder.SetReturnType(methodToOverride.ReturnType);
            methodBuilder.SetParameters(
                Seq.Make(methodParameters)
                    .Map<Type>(delegate(ParameterInfo pi) { return pi.ParameterType; })
                    .ToArray());

            int paramNum = 1;
            foreach (ParameterInfo pi in methodParameters)
            {
                methodBuilder.DefineParameter(paramNum++, pi.Attributes, pi.Name);
            }

            ILGenerator il = methodBuilder.GetILGenerator();

            LocalBuilder methodReturn = il.DeclareLocal(typeof(IMethodReturn));
            LocalBuilder ex = il.DeclareLocal(typeof(Exception));
            LocalBuilder pipeline = il.DeclareLocal(typeof(HandlerPipeline));
            LocalBuilder parameterArray = il.DeclareLocal(typeof(object[]));
            LocalBuilder inputs = il.DeclareLocal(typeof(VirtualMethodInvocation));

            // Get pipeline for this method onto the stack
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, pipelineField);
            il.Emit(OpCodes.Ldc_I4, methodToOverride.MetadataToken);
            il.EmitCall(OpCodes.Call, PipelineManagerMethods.GetPipeline, null);
            il.Emit(OpCodes.Stloc, pipeline);

            // Create instance of VirtualMethodInvocation
            il.Emit(OpCodes.Ldarg_0); // target object

            il.Emit(OpCodes.Ldtoken, methodToOverride);
            if (methodToOverride.DeclaringType.IsGenericType)
            {
                il.Emit(OpCodes.Ldtoken, methodToOverride.DeclaringType);
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
                        il.Emit(OpCodes.Box, methodParameters[i].ParameterType);
                    }
                    else if (methodParameters[i].ParameterType.IsByRef)
                    {
                        Type elementType = methodParameters[i].ParameterType.GetElementType();
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

            il.Emit(OpCodes.Ldloc, pipeline);
            il.Emit(OpCodes.Ldloc, inputs);

            // Put delegate reference onto the stack
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, delegateMethod);
            il.Emit(OpCodes.Newobj, InvokeHandlerDelegateMethods.InvokeHandlerDelegate);

            // And call the pipeline
            il.Emit(OpCodes.Call, HandlerPipelineMethods.Invoke);

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
                    il.Emit(OpCodes.Unbox_Any, ReturnType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, ReturnType);
                }
            }

            // handle byref parameters
            if (methodParameters.Length > 0)
            {
                int outArgIndex = 0;
                foreach(int parameterIndex in OutputParameterIndices)
                {
                    // Get parameter value (the address) onto the stack)
                    Type elementType = methodParameters[parameterIndex].ParameterType.GetElementType();
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
                for(int i = 0; i < methodParameters.Length; ++i)
                {
                    if(methodParameters[i].ParameterType.IsByRef)
                    {
                        yield return i;
                    }
                }
            }
        }
    }
}
