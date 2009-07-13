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
using Microsoft.Practices.Unity.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// This class records the information about which property value is currently
    /// being set, and is responsible for generating the error string required when
    /// an error has occurred.
    /// </summary>
    public class SettingPropertyOperation : PropertyOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingPropertyOperation"/> class.
        /// </summary>
        /// <param name="typeBeingConstructed">Type property is on.</param>
        /// <param name="propertyName">Name of property being set.</param>
        public SettingPropertyOperation(Type typeBeingConstructed, string propertyName) : base(typeBeingConstructed, propertyName)
        {
        }

        /// <summary>
        /// Get a format string used to create the description. Called by
        /// the base <see cref='PropertyOperation.ToString'/> method.
        /// </summary>
        /// <returns>The format string.</returns>
        protected override string GetDescriptionFormat()
        {
            return Resources.SettingPropertyOperation;
        }
    }
}
