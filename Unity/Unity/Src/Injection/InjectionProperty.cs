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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Properties;
using Guard = Microsoft.Practices.Unity.Utility.Guard;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class InjectionProperty : InjectionMember
    {
        private readonly string propertyName;
        private InjectionParameterValue parameterValue;

        /// <summary>
        /// Configure the container to inject the given property name,
        /// resolving the value via the container.
        /// </summary>
        /// <param name="propertyName">Name of the property to inject.</param>
        public InjectionProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Configure the container to inject the given property name,
        /// using the value supplied. This value is converted to an
        /// <see cref="InjectionParameterValue"/> object using the
        /// rules defined by the <see cref="InjectionParameterValue.ToParameters"/>
        /// method.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        /// <param name="propertyValue">Value for property.</param>
        public InjectionProperty(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.parameterValue = InjectionParameterValue.ToParameter(propertyValue);
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Interface being registered, ignored in this implemenation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation is done via Guard class")]
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            PropertyInfo propInfo = implementationType.GetProperty(propertyName);
            GuardPropertyExists(propInfo, implementationType, propertyName);
            GuardPropertyIsSettable(propInfo);
            GuardPropertyIsNotIndexer(propInfo);
            InitializeParameterValue(propInfo);
            GuardPropertyValueIsCompatible(propInfo, parameterValue);

            SpecifiedPropertiesSelectorPolicy selector =
                GetSelectorPolicy(policies, implementationType, name);

            selector.AddPropertyAndValue(propInfo, parameterValue);
        }

        private InjectionParameterValue InitializeParameterValue(PropertyInfo propInfo)
        {
            if (parameterValue == null)
            {
                parameterValue = new ResolvedParameter(propInfo.PropertyType);
            }
            return parameterValue;
        }

        private static SpecifiedPropertiesSelectorPolicy GetSelectorPolicy(IPolicyList policies, Type typeToInject, string name)
        {
            NamedTypeBuildKey key = new NamedTypeBuildKey(typeToInject, name);
            IPropertySelectorPolicy selector =
                policies.GetNoDefault<IPropertySelectorPolicy>(key, false);
            if (selector == null || !(selector is SpecifiedPropertiesSelectorPolicy))
            {
                selector = new SpecifiedPropertiesSelectorPolicy();
                policies.Set<IPropertySelectorPolicy>(selector, key);
            }
            return (SpecifiedPropertiesSelectorPolicy)selector;
        }

        private static void GuardPropertyExists(PropertyInfo propInfo, Type typeToCreate, string propertyName)
        {
            if (propInfo == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NoSuchProperty,
                        typeToCreate.Name,
                        propertyName));
            }
        }

        private static void GuardPropertyIsSettable(PropertyInfo propInfo)
        {
            if (!propInfo.CanWrite)
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Resources.PropertyNotSettable,
                        propInfo.Name, propInfo.DeclaringType));
            }
        }

        private static void GuardPropertyIsNotIndexer(PropertyInfo property)
        {
            if (property.GetIndexParameters().Length > 0)
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Resources.CannotInjectIndexer,
                        property.Name, property.DeclaringType));
            }
        }
        private static void GuardPropertyValueIsCompatible(PropertyInfo property, InjectionParameterValue value)
        {
            if (!value.MatchesType(property.PropertyType))
            {
                throw new InvalidOperationException(
                    ExceptionMessage(Resources.PropertyTypeMismatch,
                                     property.Name,
                                     property.DeclaringType,
                                     property.PropertyType,
                                     value.ParameterTypeName));
            }
        }

        private static string ExceptionMessage(string format, params object[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] is Type)
                {
                    args[i] = ((Type)args[i]).Name;
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }
    }
}
