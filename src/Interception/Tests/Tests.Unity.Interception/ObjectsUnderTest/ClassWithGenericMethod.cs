// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class ClassWithGenericMethod : IInterfaceWithGenericMethod
    {
        public T DoSomething<T>()
        {
            return default(T);
        }
    }
}