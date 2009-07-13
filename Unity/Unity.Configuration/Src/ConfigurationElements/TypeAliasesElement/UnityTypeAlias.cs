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
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Practices.Unity.Configuration
{
    /// <summary>
    /// A <see cref="ConfigurationElement"/> class used to manage the contents
    /// of a &lt;typeAlias&gt; node in the configuration file.
    /// </summary>
    public class UnityTypeAlias : ConfigurationElement
    {
        /// <summary>
        /// Alias to use. 
        /// </summary>
        [ConfigurationProperty("alias", IsRequired = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        /// <summary>
        /// Source type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Back compat")]
        [ConfigurationProperty("type", IsRequired = true)]
        [TypeConverter(typeof(AssemblyQualifiedTypeNameConverter))]
        public Type Type
        {
            get { return (Type)this["type"]; }
            set { this["type"] = value; }
        }
    }
}
