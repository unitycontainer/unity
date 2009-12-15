using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ParameterValueElement"/> derived class that describes
    /// a parameter that should be resolved through the container.
    /// </summary>
    public class DependencyElement : ParameterValueElement
    {
        private const string NamePropertyName = "name";
        private const string TypeNamePropertyName = "type";

        /// <summary>
        /// Create a new instance of <see cref="DependencyElement"/>.
        /// </summary>
        public DependencyElement()
        {
            
        }

        /// <summary>
        /// Create a new instance of <see cref="DependencyElement"/> with
        /// properties initialized from the contents of 
        /// <paramref name="attributeValues"/>.
        /// </summary>
        /// <param name="attributeValues">Dictionary of name/value pairs to
        /// initialize this object with.</param>
        public DependencyElement(IDictionary<string, string> attributeValues)
        {
            GuardPropertyValueIsPresent(attributeValues, "dependencyName");
            Name = attributeValues["dependencyName"];

            SetIfPresent(attributeValues, "dependencyType", value => TypeName = value);
        }

        /// <summary>
        /// Name to use to when resolving. If empty, resolves the default.
        /// </summary>
        [ConfigurationProperty(NamePropertyName, IsRequired = false)]
        public string Name
        {
            get { return (string) base[NamePropertyName]; }
            set { base[NamePropertyName] = value; }
        }

        /// <summary>
        /// Name of type this dependency should resolve to. This is optional;
        /// without it the container will resolve the type of whatever
        /// property or parameter this element is contained in.
        /// </summary>
        [ConfigurationProperty(TypeNamePropertyName, IsRequired = false)]
        public string TypeName
        {
            get { return (string) base[TypeNamePropertyName]; }
            set { base[TypeNamePropertyName] = value; }
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
            string dependencyName = Name;
            if(string.IsNullOrEmpty(dependencyName))
            {
                dependencyName = null;
            }

            return new ResolvedParameter(GetDependencyType(parameterType), dependencyName);
        }

        private Type GetDependencyType(Type parameterType)
        {
            if(!string.IsNullOrEmpty(TypeName))
            {
                return TypeResolver.ResolveType(TypeName);
            }
            return parameterType;
        }

        private static void SetIfPresent(IDictionary<string, string> attributeValues, string key, Action<string> setter)
        {
            if(attributeValues.ContainsKey(key))
            {
                setter(attributeValues[key]);
            }
        }
    }
}
