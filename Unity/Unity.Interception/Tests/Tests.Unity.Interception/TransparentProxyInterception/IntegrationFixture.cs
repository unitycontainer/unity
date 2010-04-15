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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.TransparentProxyInterception
{
    [TestClass]
    public class IntegrationFixture
    {
        [TestMethod]
        public void CanInterceptGenericMethodWithHandlerAttributeThroughInterface()
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<IInterfaceWithGenericMethod, ClassWithGenericMethod>(
                new Interceptor(new TransparentProxyInterceptor()));

            var instance = container.Resolve<IInterfaceWithGenericMethod>();

            var result = instance.DoSomething<int>();

            Assert.AreEqual(0, result);
        }

        public interface IInterfaceWithGenericMethod
        {
            [TestHandler]
            T DoSomething<T>();
        }

        public class ClassWithGenericMethod : IInterfaceWithGenericMethod
        {
            public T DoSomething<T>()
            {
                return default(T);
            }
        }

        public class TestHandlerAttribute : HandlerAttribute
        {
            public override ICallHandler CreateHandler(IUnityContainer container)
            {
                return new TestHandler();
            }
        }

        public class TestHandler : ICallHandler
        {
            #region ICallHandler Members

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var methodName = input.MethodBase.Name;
                var target = input.Target;

                return getNext()(input, getNext);
            }

            public int Order { get; set; }

            #endregion
        }


    }
}
