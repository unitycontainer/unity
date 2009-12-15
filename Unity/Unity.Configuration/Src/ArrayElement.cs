using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.Configuration.Properties;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A configuration element used to configure injection of
    /// a specific set of values into an array.
    /// </summary>
    public class ArrayElement : ParameterValueElement
    {
        private const string TypeNamePropertyName = "type";
        private const string ValuesPropertyName = "";

        /// <summary>
        /// Type of array to inject. This is actually the type of the array elements,
        /// not the array type. Optional, if not specified we take the type from
        /// our containing element.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
        }

        /// <summary>
        /// Values used to calculate the contents of the array.
        /// </summary>
        [ConfigurationProperty(ValuesPropertyName, IsDefaultCollection = true)]
        public ParameterValueElementCollection Values
        {
            get { return (ParameterValueElementCollection) base[ValuesPropertyName]; }
        }


        /// <summary>
        /// Generate an <see cref="InjectionParameterValue"/> object
        /// that will be used to configure the container for a type registration.
        /// </summary>
        /// <param name="container">Container that is being configured. Supplied in order
        /// to let custom implementations retrieve services; do not configure the container
        /// directly in this method.</param>
        /// <param name="parameterType">Type of the </param>
        /// <returns></returns>
        public override InjectionParameterValue GetInjectionParameterValue(IUnityContainer container, Type parameterType)
        {
            GuardTypeIsAnArray(parameterType);

            Type elementType = GetElementType(parameterType);

            var values = Values.Select(v => v.GetInjectionParameterValue(container, elementType));

            return new ResolvedArrayParameter(elementType, values.ToArray());
        }

        private void GuardTypeIsAnArray(Type externalParameterType)
        {
            if(string.IsNullOrEmpty(TypeName))
            {
               if(!externalParameterType.IsArray)
               {
                   throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture,
                       Resources.NotAnArray, externalParameterType.Name));
               }
            }
        }

        private Type GetElementType(Type parameterType)
        {
            return TypeResolver.ResolveTypeWithDefault(TypeName, null) ?? parameterType.GetElementType();
        }
    }
}
