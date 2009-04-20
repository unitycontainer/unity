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
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Base interface for type and instance based interceptor classes.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Can this interceptor generate a proxy for the given type?
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if interception is possible, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t")]
        bool CanIntercept(Type t);

        /// <summary>
        /// Returns a sequence of methods on the given type that can be
        /// intercepted.
        /// </summary>
        /// <param name="interceptedType">Type that was specified when this interceptor
        /// was created (typically an interface).</param>
        /// <param name="implementationType">The concrete type of the implementing object.</param>
        /// <returns>Sequence of <see cref="MethodInfo"/> objects.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Interceptable")]
        IEnumerable<MethodImplementationInfo> GetInterceptableMethods(Type interceptedType, Type implementationType);

        /// <summary>
        /// Given a <see cref="MethodImplementationInfo"/>, return the appropriate
        /// <see cref="MethodInfo"/> object to use to attach a <see cref="HandlerPipeline"/>
        /// to so that the handlers will get called when the method gets called.
        /// </summary>
        /// <param name="methodInfo">Original <see cref="MethodImplementationInfo"/> object that
        /// combines the <see cref="MethodInfo"/>s for an interface method and the corresponding implementation.</param>
        /// <returns>The <see cref="MethodInfo"/> object to pass to the <see cref="IInterceptingProxy.SetPipeline"/> method.</returns>
        MethodInfo MethodInfoForPipeline(MethodImplementationInfo methodInfo);
    }
}
