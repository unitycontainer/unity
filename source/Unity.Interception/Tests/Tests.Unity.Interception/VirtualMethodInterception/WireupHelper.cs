// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using ObjectBuilder2;

namespace Unity.InterceptionExtension.Tests.VirtualMethodInterception
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
