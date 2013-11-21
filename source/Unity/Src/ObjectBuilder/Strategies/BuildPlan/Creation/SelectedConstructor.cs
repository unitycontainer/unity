// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// Objects of this type are the return value from <see cref="IConstructorSelectorPolicy.SelectConstructor"/>.
    /// It encapsulates the desired <see cref="ConstructorInfo"/> with the string keys
    /// needed to look up the <see cref="IDependencyResolverPolicy"/> for each
    /// parameter.
    /// </summary>
    public class SelectedConstructor : SelectedMemberWithParameters<ConstructorInfo>
    {
        /// <summary>
        /// Create a new <see cref="SelectedConstructor"/> instance which
        /// contains the given constructor.
        /// </summary>
        /// <param name="constructor">The constructor to wrap.</param>
        public SelectedConstructor(ConstructorInfo constructor)
            : base(constructor)
        {
        }

        /// <summary>
        /// The constructor this object wraps.
        /// </summary>
        public ConstructorInfo Constructor
        {
            get { return MemberInfo; }
        }
    }
}
