// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest
{
    public class TestHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new TestHandler();
        }
    }
}