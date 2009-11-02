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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    internal class WireupHelper
    {
        internal static T GetInterceptingInstance<T>(params object[] ctorValues)
        {
            Type typeToIntercept = typeof(T);
            if (typeToIntercept.IsGenericType)
            {
                typeToIntercept = typeToIntercept.GetGenericTypeDefinition();
            }

            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeToIntercept);
            Type generatedType = generator.GenerateType();

            if (generatedType.IsGenericTypeDefinition)
            {
                generatedType = generatedType.MakeGenericType(typeof(T).GetGenericArguments());
            }

            return (T)Activator.CreateInstance(generatedType, ctorValues);
        }

        internal static T GetInterceptedInstance<T>(string methodName, ICallHandler handler)
        {
            MethodInfo method = typeof(T).GetMethod(methodName);

            T instance = GetInterceptingInstance<T>();

            PipelineManager manager = new PipelineManager();
            manager.SetPipeline(method, new HandlerPipeline(Sequence.Collect(handler)));

            IInterceptingProxy pm = (IInterceptingProxy)instance;
            pm.AddInterceptionBehavior(new PolicyInjectionBehavior(manager));

            return instance;
        }
    }
}
