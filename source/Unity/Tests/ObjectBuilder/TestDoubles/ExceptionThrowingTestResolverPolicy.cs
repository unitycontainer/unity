// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace ObjectBuilder2.Tests.TestDoubles
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
