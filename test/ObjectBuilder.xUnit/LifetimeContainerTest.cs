// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace ObjectBuilder2.Tests
{
     
    public class LifetimeContainerTest
    {
        public LifetimeContainerTest()
        {
            DisposeOrderCounter.ResetCount();
        }

        [Fact]
        public void CanDetermineIfLifetimeContainerContainsObject()
        {
            ILifetimeContainer container = new LifetimeContainer();
            object obj = new object();

            container.Add(obj);

            Assert.True(container.Contains(obj));
        }

        [Fact]
        public void CanEnumerateItemsInContainer()
        {
            ILifetimeContainer container = new LifetimeContainer();
            DisposableObject mdo = new DisposableObject();

            container.Add(mdo);

            int count = 0;
            bool foundMdo = false;

            foreach (object obj in container)
            {
                count++;

                if (ReferenceEquals(mdo, obj))
                {
                    foundMdo = true;
                }
            }

            Assert.Equal(1, count);
            Assert.True(foundMdo);
        }

        [Fact]
        public void ContainerEnsuresObjectsWontBeCollected()
        {
            ILifetimeContainer container = new LifetimeContainer();
            DisposableObject mdo = new DisposableObject();
            WeakReference wref = new WeakReference(mdo);

            container.Add(mdo);
            mdo = null;
            GC.Collect();

            Assert.Equal(1, container.Count);
            mdo = wref.Target as DisposableObject;
            Assert.NotNull(mdo);
            Assert.False(mdo.WasDisposed);
        }

        [Fact]
        public void DisposingContainerDisposesOwnedObjects()
        {
            ILifetimeContainer container = new LifetimeContainer();
            DisposableObject mdo = new DisposableObject();

            container.Add(mdo);
            container.Dispose();

            Assert.True(mdo.WasDisposed);
        }

        [Fact]
        public void DisposingItemsFromContainerDisposesInReverseOrderAdded()
        {
            ILifetimeContainer container = new LifetimeContainer();
            DisposeOrderCounter obj1 = new DisposeOrderCounter();
            DisposeOrderCounter obj2 = new DisposeOrderCounter();
            DisposeOrderCounter obj3 = new DisposeOrderCounter();

            container.Add(obj1);
            container.Add(obj2);
            container.Add(obj3);

            container.Dispose();

            Assert.Equal(1, obj3.DisposePosition);
            Assert.Equal(2, obj2.DisposePosition);
            Assert.Equal(3, obj1.DisposePosition);
        }

        [Fact]
        public void RemovingItemsFromContainerDoesNotDisposeThem()
        {
            ILifetimeContainer container = new LifetimeContainer();
            DisposableObject mdo = new DisposableObject();

            container.Add(mdo);
            container.Remove(mdo);
            container.Dispose();

            Assert.False(mdo.WasDisposed);
        }

        [Fact]
        public void RemovingNonContainedItemDoesNotThrow()
        {
            ILifetimeContainer container = new LifetimeContainer();

            container.Remove(new object());
        }

        private class DisposableObject : IDisposable
        {
            public bool WasDisposed = false;

            public void Dispose()
            {
                WasDisposed = true;
            }
        }

        private class DisposeOrderCounter : IDisposable
        {
            private static int count = 0;
            public int DisposePosition;

            public static void ResetCount()
            {
                count = 0;
            }

            public void Dispose()
            {
                DisposePosition = ++count;
            }
        }
    }
}
