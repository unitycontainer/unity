// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class that wraps the inputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    [SecurityCritical]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    class TransparentProxyInputParameterCollection : ParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="TransparentProxyInputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public TransparentProxyInputParameterCollection(IMethodCallMessage callMessage, object[] arguments)
            : base(arguments, callMessage.MethodBase.GetParameters(),
                delegate(ParameterInfo info) { return !info.IsOut; })
        {
        }
    }
}
