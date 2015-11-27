// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public interface IInterfaceWithGenericMethod
    {
        [TestHandler]
        T DoSomething<T>();
    }
}