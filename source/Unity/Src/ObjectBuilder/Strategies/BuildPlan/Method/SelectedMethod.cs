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

using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Objects of this type are the return value from <see cref="IMethodSelectorPolicy.SelectMethods"/>.
    /// It encapsulates the desired <see cref="MethodInfo"/> with the string keys
    /// needed to look up the <see cref="IDependencyResolverPolicy"/> for each
    /// parameter.
    /// </summary>
    public class SelectedMethod : SelectedMemberWithParameters<MethodInfo>
    {
        /// <summary>
        /// Create a new <see cref="SelectedMethod"/> instance which
        /// contains the given method.
        /// </summary>
        /// <param name="method">The method</param>
        public SelectedMethod(MethodInfo method)
            : base(method)
        {
        }

        /// <summary>
        /// The constructor this object wraps.
        /// </summary>
        public MethodInfo Method
        {
            get { return MemberInfo; }
        }
    }
}
