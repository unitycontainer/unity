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

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Tests;
using Unity.ObjectBuilder;
using ObjectBuilder2;

namespace Microsoft.Practices.Unity.Tests
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
                    catch(ResolutionFailedException)
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
                Thread.Sleep(delayMS);
                delayMS = delayMS == 0 ? 500 : 0;
            }
        }

        // Another test strategy that throws an exeception the
        // first time it is executed.
        private class ThrowingStrategy : BuilderStrategy
        {
            private bool shouldThrow = true;


            public override void PreBuildUp(IBuilderContext context)
            {
                if (shouldThrow)
                {
                    shouldThrow = false;
                    throw new Exception("Throwing from buildup chain");
                }
            }
        }
    }
}
