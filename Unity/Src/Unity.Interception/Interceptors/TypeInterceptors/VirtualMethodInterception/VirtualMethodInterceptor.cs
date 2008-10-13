using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A type based interceptor that works by generated a new class
    /// on the fly that derives from the target class.
    /// </summary>
    public class VirtualMethodInterceptor : ITypeInterceptor
    {
        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done via Guard class")]
        public bool CanIntercept(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            return t.IsClass &&
                (t.IsPublic || t.IsNestedPublic) &&
                t.IsVisible &&
                !t.IsAbstract &&
                !t.IsSealed;
        }

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done via Guard class")]
        public IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType)
        {
            Guard.ArgumentNotNull(interceptedType, "interceptedType");
            Guard.ArgumentNotNull(implementationType, "implementationType");
            foreach (MethodInfo method in implementationType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if(MethodOverride.MethodCanBeIntercepted(method))
                {
                    yield return new MethodImplementationInfo(null, method);
                }
            }
        }

        /// <summary>
        /// Create a type to proxy for the given type <paramref name="t"/>.
        /// </summary>
        /// <param name="t">Type to proxy.</param>
        /// <returns>New type that can be instantiated instead of the
        /// original type t, and supports interception.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification="Validation done via Guard class")]
        public Type CreateProxyType(Type t)
        {
            Guard.ArgumentNotNull(t, "t");
            if(!CanIntercept(t))
            {
                throw new InvalidOperationException(Resources.InterceptionNotSupported);
            }
            InterceptingClassGenerator classGenerator = new InterceptingClassGenerator(t, GetInterceptableMethods(t, t));
            return classGenerator.GenerateType();
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
