// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class that wraps the outputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    class TransparentProxyOutputParameterCollection : ParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="TransparentProxyOutputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public TransparentProxyOutputParameterCollection(IMethodCallMessage callMessage, object[] arguments)
            : base(arguments, callMessage.MethodBase.GetParameters(), parameterInfo => parameterInfo.ParameterType.IsByRef)
        {
        }
    }
}
