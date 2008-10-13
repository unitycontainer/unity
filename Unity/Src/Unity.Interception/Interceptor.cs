using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An abstract class that is used to configure interception
    /// of various kinds for each method. Can be passed either
    /// in the RegisterType call or in the methods of the <see cref="Interception"/>
    /// extension.
    /// </summary>
    public abstract class Interceptor : InjectionMember
    {
        /// <summary>
        /// A method that encapsulates the logic used to find the particular
        /// interception policy used by the various strategies. It handles
        /// looking for the various defaults.
        /// </summary>
        /// <typeparam name="TPolicy">Policy type you're looking for</typeparam>
        /// <returns>The policy object or null if not found.</returns>
        public static TPolicy FindInterceptorPolicy<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            return FindPolicyForCurrentKey<TPolicy>(context) ??
                FindPolicyForCurrentKeyType<TPolicy>(context) ??
                    FindPolicyForOriginalKey<TPolicy>(context) ??
                        FindPolicyForOriginalKeyType<TPolicy>(context);
        }

        private static TPolicy FindPolicyForCurrentKey<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            return context.Policies.GetNoDefault<TPolicy>(context.BuildKey, false);
        }

        private static TPolicy FindPolicyForCurrentKeyType<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            Type typeToBuild;
            TPolicy policy = null;
            if(BuildKey.TryGetType(context.BuildKey, out typeToBuild))
            {
                policy = context.Policies.GetNoDefault<TPolicy>(typeToBuild, false);
            }
            return policy;
        }

        private static TPolicy FindPolicyForOriginalKey<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            return context.Policies.GetNoDefault<TPolicy>(context.OriginalBuildKey, false);
        }

        private static TPolicy FindPolicyForOriginalKeyType<TPolicy>(IBuilderContext context)
            where TPolicy : class, IBuilderPolicy
        {
            Type typeToBuild;
            TPolicy policy = null;
            if(BuildKey.TryGetType(context.OriginalBuildKey, out typeToBuild))
            {
                policy = context.Policies.GetNoDefault<TPolicy>(typeToBuild, false);
            }
            return policy;
        }
    }
}
