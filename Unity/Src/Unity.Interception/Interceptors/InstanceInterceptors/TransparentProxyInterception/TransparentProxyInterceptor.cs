using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Text;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An instance interceptor that uses remoting proxies to do the
    /// interception.
    /// </summary>
    public class TransparentProxyInterceptor : IInstanceInterceptor
    {
        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done by Guard class")]
        public bool CanIntercept(Type t)
        {
            Guard.ArgumentNotNull(t, "t");

            return (typeof(MarshalByRefObject).IsAssignableFrom(t) || t.IsInterface);
        }

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            if(typeof(MarshalByRefObject).IsAssignableFrom(implementationType))
            {
                return GetMBROMethods(implementationType);
            }

            return GetImplementedInterfaceMethods(implementationType);
        }

        private static IEnumerable<MethodImplementationInfo> GetMBROMethods(Type t)
        {
            Dictionary<MethodInfo, bool> haveSeenMethod = new Dictionary<MethodInfo, bool>();

            foreach(MethodImplementationInfo methodImpl in GetImplementedInterfaceMethods(t))
            {
                haveSeenMethod[methodImpl.ImplementationMethodInfo] = true;
                yield return methodImpl;
            }

            foreach(MethodInfo method in t.GetMethods())
            {
                if (IsNotSystemMethod(method) && !haveSeenMethod.ContainsKey(method))
                {
                    yield return new MethodImplementationInfo(null, method);
                }
            }
        }

        private static IEnumerable<MethodImplementationInfo> GetImplementedInterfaceMethods(Type t)
        {

            foreach(Type itf in t.GetInterfaces())
            {
                InterfaceMapping mapping = t.GetInterfaceMap(itf);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i )
                {
                    yield return new MethodImplementationInfo(
                        mapping.InterfaceMethods[i], mapping.TargetMethods[i]);
                }
            }
        }

        private static bool IsNotSystemMethod(MethodInfo method)
        {
            return method.DeclaringType != typeof (MarshalByRefObject) && method.DeclaringType != typeof (object);
        }
        /// <summary>
        /// Create a proxy object that provides interception for <paramref name="target"/>.
        /// </summary>
        /// <param name="t">Type to generate the proxy of.</param>
        /// <param name="target">Object to create the proxy for.</param>
        /// <returns>The proxy object.</returns>
        public IInterceptingProxy CreateProxy(Type t, object target)
        {
            Guard.ArgumentNotNull(t, "t");
            Guard.ArgumentNotNull(target, "target");

            RealProxy realProxy = new InterceptingRealProxy(target, t);
            return (IInterceptingProxy) realProxy.GetTransparentProxy();
        }

        /// <summary>
        /// Given a <see cref="MethodImplementationInfo"/>, return the appropriate
        /// <see cref="MethodInfo"/> object to use to attach a <see cref="HandlerPipeline"/>
        /// to so that the handlers will get called when the method gets called.
        /// </summary>
        /// <param name="methodInfo">Original <see cref="MethodImplementationInfo"/> object that
        /// combines the <see cref="MethodInfo"/>s for an interface method and the corresponding implementation.</param>
        /// <returns>The <see cref="MethodInfo"/> object to pass to the <see cref="IInterceptingProxy.SetPipeline"/> method.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via Guard class.")]
        public MethodInfo MethodInfoForPipeline(MethodImplementationInfo methodInfo)
        {
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            return methodInfo.ImplementationMethodInfo;
        }
    }
}