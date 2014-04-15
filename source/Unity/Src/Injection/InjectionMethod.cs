// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : InjectionMember
    {
        private readonly string methodName;
        private readonly List<InjectionParameterValue> methodParameters;

        /// <summary>
        /// Create a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given methods with the given parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="methodParameters">Parameter values for the method.</param>
        public InjectionMethod(string methodName, params object[] methodParameters)
        {
            this.methodName = methodName;
            this.methodParameters = InjectionParameterValue.ToParameters(methodParameters).ToList();
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Type of interface registered, ignored in this implementation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            MethodInfo methodInfo = this.FindMethod(implementationType);
            this.ValidateMethodCanBeInjected(methodInfo, implementationType);

            SpecifiedMethodsSelectorPolicy selector =
                GetSelectorPolicy(policies, implementationType, name);
            selector.AddMethodAndParameters(methodInfo, this.methodParameters);
        }

        /// <summary>
        /// A small function to handle name matching. You can override this
        /// to do things like case insensitive comparisons.
        /// </summary>
        /// <param name="targetMethod">MethodInfo for the method you're checking.</param>
        /// <param name="nameToMatch">Name of the method you're looking for.</param>
        /// <returns>True if a match, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        protected virtual bool MethodNameMatches(MemberInfo targetMethod, string nameToMatch)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(targetMethod, "targetMethod");

            return targetMethod.Name == nameToMatch;
        }

        private MethodInfo FindMethod(Type typeToCreate)
        {
            ParameterMatcher matcher = new ParameterMatcher(this.methodParameters);
            foreach (MethodInfo method in typeToCreate.GetMethodsHierarchical())
            {
                if (this.MethodNameMatches(method, this.methodName))
                {
                    if (matcher.Matches(method.GetParameters()))
                    {
                        return method;
                    }
                }
            }
            return null;
        }

        private void ValidateMethodCanBeInjected(MethodInfo method, Type typeToCreate)
        {
            this.GuardMethodNotNull(method, typeToCreate);
            this.GuardMethodNotStatic(method, typeToCreate);
            this.GuardMethodNotGeneric(method, typeToCreate);
            this.GuardMethodHasNoOutParams(method, typeToCreate);
            this.GuardMethodHasNoRefParams(method, typeToCreate);
        }

        private void GuardMethodNotNull(MethodInfo info, Type typeToCreate)
        {
            if (info == null)
            {
                this.ThrowIllegalInjectionMethod(Resources.NoSuchMethod, typeToCreate);
            }
        }

        private void GuardMethodNotStatic(MethodInfo info, Type typeToCreate)
        {
            if (info.IsStatic)
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectStaticMethod, typeToCreate);
            }
        }

        private void GuardMethodNotGeneric(MethodInfo info, Type typeToCreate)
        {
            if (info.IsGenericMethodDefinition)
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectGenericMethod, typeToCreate);
            }
        }

        private void GuardMethodHasNoOutParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any(param => param.IsOut))
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithOutParams, typeToCreate);
            }
        }

        private void GuardMethodHasNoRefParams(MethodInfo info, Type typeToCreate)
        {
            if (info.GetParameters().Any(param => param.ParameterType.IsByRef))
            {
                this.ThrowIllegalInjectionMethod(Resources.CannotInjectMethodWithRefParams, typeToCreate);
            }
        }

        private void ThrowIllegalInjectionMethod(string message, Type typeToCreate)
        {
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    message,
                    typeToCreate.GetTypeInfo().Name,
                    this.methodName,
                    this.methodParameters.JoinStrings(", ", mp => mp.ParameterTypeName)));
        }

        private static SpecifiedMethodsSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToCreate, string name)
        {
            var key = new NamedTypeBuildKey(typeToCreate, name);
            var selector = policies.GetNoDefault<IMethodSelectorPolicy>(key, false);
            if (selector == null || !(selector is SpecifiedMethodsSelectorPolicy))
            {
                selector = new SpecifiedMethodsSelectorPolicy();
                policies.Set<IMethodSelectorPolicy>(selector, key);
            }
            return (SpecifiedMethodsSelectorPolicy)selector;
        }
    }
}
