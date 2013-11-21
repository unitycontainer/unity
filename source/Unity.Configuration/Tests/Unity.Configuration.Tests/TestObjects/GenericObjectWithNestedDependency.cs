// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class GenericObjectWithNestedDependency<T>
    {
        public GenericObjectWithConstructorDependency<T> Value { get; private set; }

        public GenericObjectWithNestedDependency(GenericObjectWithConstructorDependency<T> value)
        {
            Value = value;
        }
    }
}
