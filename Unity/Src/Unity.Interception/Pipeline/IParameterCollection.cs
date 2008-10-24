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

using System.Collections;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This interface represents a list of either input or output
    /// parameters. It implements a fixed size list, plus a couple
    /// of other utility methods.
    /// </summary>
    public interface IParameterCollection : IList
    {
        /// <summary>
        /// Fetches a parameter's value by name.
        /// </summary>
        /// <param name="paramName">parameter name.</param>
        /// <returns>value of the named parameter.</returns>
        object this[string paramName] { get; set; }

        /// <summary>
        /// Gets the name of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the name for.</param>
        /// <returns>Name of the requested parameter.</returns>
        string ParameterName(int index);

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by index.
        /// </summary>
        /// <param name="index">Index for this parameter.</param>
        /// <returns>ParameterInfo object describing the parameter.</returns>
        ParameterInfo GetParameterInfo(int index);

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by name.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>ParameterInfo object for the named parameter.</returns>
        ParameterInfo GetParameterInfo(string paramName);
    }
}
