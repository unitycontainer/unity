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
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that generates IL to call
    /// chosen methods (as specified by the current <see cref="IMethodSelectorPolicy"/>)
    /// as part of object build up.
    /// </summary>
    public class DynamicMethodCallStrategy : BuilderStrategy
    {
        private static MethodInfo throwOnParameterResolveFailed =
            typeof(DynamicMethodCallStrategy).GetMethod("ThrowOnParameterResolveFailed");

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        // FxCop suppression: Validation is done by Guard class
        public override void PreBuildUp(IBuilderContext context)
        {
            DynamicBuildPlanGenerationContext ilContext = (DynamicBuildPlanGenerationContext)(context.Existing);
            IMethodSelectorPolicy selector = context.Policies.Get<IMethodSelectorPolicy>(context.BuildKey);

            LocalBuilder currentMethodName = ilContext.IL.DeclareLocal(typeof(string));
            LocalBuilder currentParameterName = ilContext.IL.DeclareLocal(typeof(string));

            ilContext.IL.BeginExceptionBlock();

            foreach (SelectedMethod method in selector.SelectMethods(context))
            {
                ilContext.IL.Emit(OpCodes.Ldstr, GetMethodSignature(method.Method));
                ilContext.IL.Emit(OpCodes.Stloc, currentMethodName);
                ilContext.IL.Emit(OpCodes.Ldnull);
                ilContext.IL.Emit(OpCodes.Stloc, currentParameterName);

                GuardMethodIsNotOpenGeneric(method.Method);
                GuardMethodHasNoOutParams(method.Method);
                GuardMethodHasNoRefParams(method.Method);

                ParameterInfo[] parameters = method.Method.GetParameters();

                ilContext.EmitLoadExisting();

                int i = 0;
                foreach(string key in method.GetParameterKeys())
                {
                    ilContext.IL.Emit(OpCodes.Ldstr, parameters[i].Name);
                    ilContext.IL.Emit(OpCodes.Stloc, currentParameterName);
                    ilContext.EmitResolveDependency(parameters[i].ParameterType, key);
                    ++i;
                }
                ilContext.IL.Emit(OpCodes.Ldnull);
                ilContext.IL.Emit(OpCodes.Stloc, currentParameterName);
                ilContext.IL.EmitCall(OpCodes.Callvirt, method.Method, null);
                if(method.Method.ReturnType != typeof(void))
                {
                    ilContext.IL.Emit(OpCodes.Pop);
                }
            }

            ilContext.IL.BeginCatchBlock(typeof(Exception));

            Label parameterResolveFailed = ilContext.IL.DefineLabel();
            ilContext.IL.Emit(OpCodes.Ldloc, currentParameterName);
            ilContext.IL.Emit(OpCodes.Brtrue, parameterResolveFailed);

            // Failure was in the method call.
            ilContext.IL.Emit(OpCodes.Rethrow);

            ilContext.IL.MarkLabel(parameterResolveFailed);
            ilContext.IL.Emit(OpCodes.Ldloc, currentMethodName);
            ilContext.IL.Emit(OpCodes.Ldloc, currentParameterName);
            ilContext.IL.EmitCall(OpCodes.Call, throwOnParameterResolveFailed, null);

            ilContext.IL.EndExceptionBlock();
        }

        private static void GuardMethodIsNotOpenGeneric(MethodInfo method)
        {
            if(method.IsGenericMethodDefinition)
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectOpenGenericMethod, method);
            }
        }

        private static void GuardMethodHasNoOutParams(MethodInfo method)
        {
            if(Array.Find(method.GetParameters(), 
                delegate(ParameterInfo param)
                {
                    return param.IsOut;
                }) != null)
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void GuardMethodHasNoRefParams(MethodInfo method)
        {
            if (Array.Find(method.GetParameters(),
                delegate(ParameterInfo param)
                {
                    return param.ParameterType.IsByRef;
                }) != null)
            {
                ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParam, method);
            }
        }

        private static void ThrowIllegalInjectionMethod(string format, MethodInfo method)
        {
                throw new IllegalInjectionMethodException(
                    string.Format(CultureInfo.CurrentCulture,
                        format,
                        method.DeclaringType.Name,
                        method.Name));
            
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception
        /// when an injection method parameter could not be resolved.
        /// </summary>
        /// <param name="inner">Exception that provides the failure info.</param>
        /// <param name="methodName">Name of the method that was going to be called.</param>
        /// <param name="parameterName">Parameter that failed to resolve.</param>
        public static void ThrowOnParameterResolveFailed(Exception inner, string methodName, string parameterName)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.MethodParameterResolutionFailed,
                    methodName,
                    parameterName),
                inner);
        }

        private static string GetMethodSignature(MethodBase method)
        {
            string typeName = method.DeclaringType.FullName;
            string methodName = method.Name;
            ParameterInfo[] parameterInfos = method.GetParameters();
            string[] parameterDescriptions = new string[parameterInfos.Length];

            for(int i = 0; i < parameterInfos.Length; ++i)
            {
                parameterDescriptions[i] = parameterInfos[i].ParameterType.FullName + " " +
                    parameterInfos[i].Name;
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}.{1}({2})",
                typeName,
                methodName,
                string.Join(", ", parameterDescriptions));
        }
    }
}



