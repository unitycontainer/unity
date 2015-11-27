// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.Configuration.Tests.TestObjects
{
    internal class GenericObjectWithNestedDependency<T>
    {
        public GenericObjectWithConstructorDependency<T> Value { get; private set; }

        public GenericObjectWithNestedDependency(GenericObjectWithConstructorDependency<T> value)
        {
            Value = value;
        }
    }
}
