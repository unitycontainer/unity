using System;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    internal class WireupHelper
    {
        internal static T GetInterceptingInstance<T>(params object[] ctorValues )
        {
            VirtualMethodInterceptor interceptor = new VirtualMethodInterceptor();
            Type typeToIntercept = typeof (T);
            if(typeToIntercept.IsGenericType)
            {
                typeToIntercept = typeToIntercept.GetGenericTypeDefinition();
            }

            InterceptingClassGenerator generator = new InterceptingClassGenerator(typeToIntercept, interceptor.GetInterceptableMethods(typeToIntercept, typeToIntercept));
            Type generatedType = generator.GenerateType();

            if(generatedType.IsGenericTypeDefinition)
            {
                generatedType = generatedType.MakeGenericType(typeof (T).GetGenericArguments());
            }

            return (T) Activator.CreateInstance(generatedType, ctorValues);
        }

        internal static T GetInterceptedInstance<T>(string methodName, ICallHandler handler)
        {
            MethodInfo method = typeof (T).GetMethod(methodName);

            T instance = GetInterceptingInstance<T>();

            IInterceptingProxy pm = (IInterceptingProxy) instance;
            pm.SetPipeline(method, new HandlerPipeline(Seq.Collect(handler)));

            return instance;
        }
    }
}
