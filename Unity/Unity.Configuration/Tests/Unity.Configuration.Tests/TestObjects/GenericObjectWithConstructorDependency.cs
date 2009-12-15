using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects
{
    class GenericObjectWithConstructorDependency<T>
    {
        public T Value { get; private set; }

        public GenericObjectWithConstructorDependency(T value)
        {
            Value = value;
        }
    }
}
