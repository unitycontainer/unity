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
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Tests.TestDoubles;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

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

            Task task1 = new Task(delegate()
            {
                result1 = container.Resolve<object>();
            });
            
            Task task2 = new Task(delegate()
            {
                result2 = container.Resolve<object>();
            });

            task1.Start();
            task2.Start();

            Task.WaitAll(task1, task2);

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

            Task task1 = new Task(
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

            Task task2 = new Task(
                delegate()
                {
                    result2 = container.Resolve<object>();
                    thread2Finished = true;
                });


            task1.Start();
            task1.Wait();
            

            // Thread1 threw an exception. However, lock should be correctly freed.
            // Run thread2, and if it finished, we're ok.

            task2.Start();
            task2.Wait(1000);

            Assert.IsTrue(thread2Finished);
            Assert.IsNull(result1);
            Assert.IsNotNull(result2);
        }

        // A test strategy that introduces a variable delay in
        // the strategy chain to work out 
        private class DelayStrategy : BuilderStrategy
        {
            private int delayMS = 500;
            private static object @lock = new object();

            public override void PreBuildUp(IBuilderContext context)
            {
                lock (@lock)
                {
                    SpinWait.SpinUntil(() => false, delayMS);
                }

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
