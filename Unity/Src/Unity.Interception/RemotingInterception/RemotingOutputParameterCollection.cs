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
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class that wraps the outputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    class RemotingOutputParameterCollection : ParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="RemotingOutputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public RemotingOutputParameterCollection(IMethodCallMessage callMessage, object[] arguments)
            : base(arguments, callMessage.MethodBase.GetParameters(),
                delegate(ParameterInfo info) { return info.IsOut; })
        {
        }
    }
}
