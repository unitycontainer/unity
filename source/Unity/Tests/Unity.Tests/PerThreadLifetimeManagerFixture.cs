// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using Microsoft.Practices.ObjectBuilder2;
#if NETFX_CORE
#if !WINDOWS_PHONE
using System;
using System.Threading.Tasks;
using Windows.System.Threading;
#endif
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class PerThreadLifetimeManagerFixture
    {
        [TestMethod]
        public void CanCreateLifetimeManager()
        {
            new PerThreadLifetimeManager();
        }

        [TestMethod]
        public void NewLifetimeManagerReturnsNullForObject()
        {
            LifetimeManager ltm = new PerThreadLifetimeManager();
            Assert.IsNull(ltm.GetValue());
        }

        [TestMethod]
        public void LifetimeManagerReturnsValueThatWasSetOnSameThread()
        {
            LifetimeManager ltm = new PerThreadLifetimeManager();
            string expected = "Here's the value";

            ltm.SetValue(expected);
            object result = ltm.GetValue();
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void DifferentLifetimeContainerInstancesHoldDifferentObjects()
        {
            LifetimeManager ltm1 = new PerThreadLifetimeManager();
            LifetimeManager ltm2 = new PerThreadLifetimeManager();
            string expected1 = "Here's the first value";
            string expected2 = "Here's the second value";

            ltm1.SetValue(expected1);
            ltm2.SetValue(expected2);

            object result1 = ltm1.GetValue();
            object result2 = ltm2.GetValue();
            Assert.AreSame(expected1, result1);
            Assert.AreSame(expected2, result2);
        }

        [TestMethod]
        public void LifetimeManagerReturnsNullIfCalledOnADifferentThreadFromTheOneThatSetTheValue()
        {
            LifetimeManager ltm = new PerThreadLifetimeManager();
            string expected = "Here's the value";

            ltm.SetValue(expected);

            // Providing dummy initializers so we can prove the values are different coming out of the LTM
            var otherThreadResult = new object();

            RunInParallel(() => { otherThreadResult = ltm.GetValue(); });

            Assert.AreSame(expected, ltm.GetValue());
            Assert.IsNull(otherThreadResult);
        }

        [TestMethod]
        public void LifetimeManagerReturnsDifferentValuesForEachThread()
        {
            LifetimeManager ltm = new PerThreadLifetimeManager();
            string one = "one";
            string two = "two";
            string three = "three";

            object valueOne = null;
            object valueTwo = null;
            object ValueThree = null;

            var barrier = new Barrier(3);
            RunInParallel(
                delegate { ltm.SetValue(one); barrier.SignalAndWait(); valueOne = ltm.GetValue(); },
                delegate { ltm.SetValue(three); barrier.SignalAndWait(); ValueThree = ltm.GetValue(); },
                delegate { ltm.SetValue(two); barrier.SignalAndWait(); valueTwo = ltm.GetValue(); });

            Assert.AreSame(one, valueOne);
            Assert.AreSame(two, valueTwo);
            Assert.AreSame(three, ValueThree);
        }

        [TestMethod]
        public void CanRegisterLifetimeManagerInContainerAndUseItOnOneThread()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<object>(new PerThreadLifetimeManager());

            object result1 = container.Resolve<object>();
            object result2 = container.Resolve<object>();

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void ReturnsDifferentObjectsOnDifferentThreadsFromContainer()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<object>(new PerThreadLifetimeManager());

            object result1 = null;
            object result2 = null;

            RunInParallel(
                delegate { result1 = container.Resolve<object>(); },
                delegate { result2 = container.Resolve<object>(); });

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);

            Assert.AreNotSame(result1, result2);
        }

        [TestMethod]
        public void RegisteringAnInstanceInAThreadSetsPerThreadLifetimeManagerWhenResolvingInOtherThreads()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<object>(new PerThreadLifetimeManager());
            LifetimeManager manager = new PerThreadLifetimeManager();

            object registered = new object();
            object result1A = null;
            object result1B = null;
            object result2A = null;
            object result2B = null;

            container.RegisterInstance(registered, manager);

            var barrier = new Barrier(2);
            RunInParallel(
                delegate
                {
                    result1A = container.Resolve<object>();
                    barrier.SignalAndWait();
                    result1B = container.Resolve<object>();
                },
                delegate
                {
                    result2A = container.Resolve<object>();
                    barrier.SignalAndWait();
                    result2B = container.Resolve<object>();
                });
            object result = container.Resolve<object>();

            Assert.IsNotNull(result1A);
            Assert.IsNotNull(result2A);
            Assert.IsNotNull(result);

            Assert.AreNotSame(result1A, result2A);
            Assert.AreNotSame(registered, result1A);
            Assert.AreNotSame(registered, result2A);
            Assert.AreSame(result1A, result1B);
            Assert.AreSame(result2A, result2B);
            Assert.AreSame(registered, result);
        }

#if NETFX_CORE && !WINDOWS_PHONE
        // Helper method to run a bunch of delegates, each on a separate thread.
        // It runs them and then waits for them all to complete.
        private static void RunInParallel(params System.Action[] actions)
        {
            var barrier = new Barrier(actions.Length);
            var tasks = actions.Select(a =>
                    ThreadPool.RunAsync(
                        _ =>
                        {
                            barrier.SignalAndWait();
                            a();
                        }).AsTask()).ToArray();

            Task.WaitAll(tasks);
        }
#else
        // Helper method to run a bunch of delegates, each on a separate thread.
        // It runs them and then waits for them all to complete.
        private static void RunInParallel(params ThreadStart[] actions)
        {
            // We use explicit threads here rather than delegate.BeginInvoke
            // because the latter uses the threadpool, and could reuse thread
            // pool threads depending on timing. We want to guarantee different
            // threads for these tests.

            Thread[] threads = actions.Select(a => new Thread(a)).ToArray();

            // Start them all...
            threads.ForEach(t => t.Start());

            // And wait for them all to finish
            threads.ForEach(t => t.Join());
        }
#endif
    }
}
