// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Threading;
using Microsoft.Practices.Unity;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Unity.Tests.Generics;

namespace Unity.Tests.Lifetime
{
    [TestClass]
    public class PerThreadLifeTimeManagerFixture
    {
        [TestMethod]
        public void ContainerReturnsTheSameInstanceOnTheSameThread()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IHaveManyGenericTypesClosed, HaveManyGenericTypesClosed>(new PerThreadLifetimeManager());

            IHaveManyGenericTypesClosed a = container.Resolve<IHaveManyGenericTypesClosed>();
            IHaveManyGenericTypesClosed b = container.Resolve<IHaveManyGenericTypesClosed>();

            Assert.AreSame(a, b);
        }

        [TestMethod]
        public void ContainerReturnsDifferentInstancesOnDifferentThreads()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IHaveManyGenericTypesClosed, HaveManyGenericTypesClosed>(new PerThreadLifetimeManager());

            Thread t1 = new Thread(new ParameterizedThreadStart(ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadProcedure));
            Thread t2 = new Thread(new ParameterizedThreadStart(ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadProcedure));

            ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation info =
                new ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation(container);

            t1.Start(info);
            t2.Start(info);
            t1.Join();
            t2.Join();

            IHaveManyGenericTypesClosed a = new List<IHaveManyGenericTypesClosed>(info.ThreadResults.Values)[0];
            IHaveManyGenericTypesClosed b = new List<IHaveManyGenericTypesClosed>(info.ThreadResults.Values)[1];

            Assert.AreNotSame(a, b);
        }

        public class ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation
        {
            private readonly IUnityContainer _container;
            private readonly Dictionary<Thread, IHaveManyGenericTypesClosed> _threadResults;
            private readonly object dictLock = new object();

            public ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation(IUnityContainer container)
            {
                _container = container;
                _threadResults = new Dictionary<Thread, IHaveManyGenericTypesClosed>();
            }

            public IUnityContainer Container
            {
                get { return _container; }
            }

            public Dictionary<Thread, IHaveManyGenericTypesClosed> ThreadResults
            {
                get { return _threadResults; }
            }

            public void SetThreadResult(Thread t, IHaveManyGenericTypesClosed result)
            {
                lock (dictLock)
                {
                    _threadResults.Add(t, result);
                }
            }
        }

        private void ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadProcedure(object o)
        {
            ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation info = o as ContainerReturnsDifferentInstancesOnDifferentThreads_ThreadInformation;

            IHaveManyGenericTypesClosed resolve1 = info.Container.Resolve<IHaveManyGenericTypesClosed>();
            IHaveManyGenericTypesClosed resolve2 = info.Container.Resolve<IHaveManyGenericTypesClosed>();

            Assert.AreSame(resolve1, resolve2);

            info.SetThreadResult(Thread.CurrentThread, resolve1);
        }
    }
}
