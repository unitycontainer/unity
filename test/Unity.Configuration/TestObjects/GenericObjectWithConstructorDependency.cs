// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Configuration.Tests.TestObjects
{
    internal class GenericObjectWithConstructorDependency<T>
    {
        public T Value { get; private set; }

        public GenericObjectWithConstructorDependency(T value)
        {
            Value = value;
        }
    }
}
