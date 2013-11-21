// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Class containing information about a type name.
    /// </summary>
    internal class TypeNameInfo
    {
        /// <summary>
        /// The base name of the class
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Namespace if any
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Assembly name, if any
        /// </summary>
        public string AssemblyName { get; set; }

        public bool IsGenericType { get; set; }

        public bool IsOpenGeneric { get; set; }

        public int NumGenericParameters { get { return GenericParameters.Count; } }

        public List<TypeNameInfo> GenericParameters { get; private set; }

        public TypeNameInfo()
        {
            GenericParameters = new List<TypeNameInfo>();
        }

        public string FullName
        {
            get
            {
                string name = Name;
                if(IsGenericType)
                {
                    name += '`' + NumGenericParameters.ToString(CultureInfo.InvariantCulture);
                }
                if(!string.IsNullOrEmpty(Namespace))
                {
                    name = Namespace + '.' + name;
                }
                if(!string.IsNullOrEmpty(AssemblyName))
                {
                    name = name + ", " + AssemblyName;
                }
                return name;
            }
        }
    }
}
