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

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles
{
    public class ExceptionThrowingTestResolverPolicy : IDependencyResolverPolicy
    {
        private Exception exceptionToThrow;

        public ExceptionThrowingTestResolverPolicy(Exception exceptionToThrow)
        {
            this.exceptionToThrow = exceptionToThrow;
        }

        public object Resolve(IBuilderContext context)
        {
            throw this.exceptionToThrow;
        }
    }
}
