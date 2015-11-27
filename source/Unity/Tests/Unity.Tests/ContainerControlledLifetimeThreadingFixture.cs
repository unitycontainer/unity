// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using Unity.Tests.TestDoubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    // Test for a race condition in the ContainerControlledLifetime
    // class.
    [TestClass]
    public class ContainerControlledLifetimeThreadingFixture
    {
        [TestMethod]
        public void ContainerControlledLifetimeReturnsSameInstanceFromMultipleThreads()
        {
            IUnityContainer container = new UnityContainer()
                .AddExtension(new SpyExtension(new DelayStrategy(), UnityBuildStage.Lifetime))
                .RegisterType<object>(new ContainerControlledLifetimeManager());

            object result1 = null;
            object result2 = null;

            Thread thread1 = new Thread(delegate()
            {
                result1 = container.Resolve<object>();
            });

            Thread thread2 = new Thread(delegate()
            {
                result2 = container.Resolve<object>();
            });

            thread1.Start();
            thread2.Start();

            thread2.Join();
            thread1.Join();

            Assert.IsNotNull(result1);
            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void ContainerControlledLifetimeDoesNotLeaveHangingLockIfBuildThrowsException()
        {
            IUnityContainer container = new UnityContainer()
                .AddExtension(new SpyExtension(new ThrowingStrategy(), UnityBuildStage.PostInitialization))
                .RegisterType<object>(new ContainerControlledLifetimeManager());

            object result1 = null;
            object result2 = null;
            bool thread2Finished = false;

            Thread thread1 = new Thread(
                delegate()
                {
                    try
                    {
                        result1 = container.Resolve<object>();
                    }
                    catch (ResolutionFailedException)
                    {
                    }
                });

            Thread thread2 = new Thread(
                delegate()
                {
                    result2 = container.Resolve<object>();
                    thread2Finished = true;
                });

            thread1.Start();
            thread1.Join();

            // Thread1 threw an exception. However, lock should be correctly freed.
            // Run thread2, and if it finished, we're ok.

            thread2.Start();
            thread2.Join(1000);

            Assert.IsTrue(thread2Finished);
            Assert.IsNull(result1);
            Assert.IsNotNull(result2);
        }

        // A test strategy that introduces a variable delay in
        // the strategy chain to work out 
        private class DelayStrategy : BuilderStrategy
        {
            private int delayMS = 500;

            public override void PreBuildUp(IBuilderContext context)
            {
                Thread.Sleep(this.delayMS);
                this.delayMS = this.delayMS == 0 ? 500 : 0;
            }
        }

        // Another test strategy that throws an exeception the
        // first time it is executed.
        private class ThrowingStrategy : BuilderStrategy
        {
            private bool shouldThrow = true;

            public override void PreBuildUp(IBuilderContext context)
            {
                if (this.shouldThrow)
                {
                    this.shouldThrow = false;
                    throw new Exception("Throwing from buildup chain");
                }
            }
        }
    }
}
