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

namespace Unity.Tests
{
     
    public class DisposableExtensionFixture
    {
        [Fact]
        public void DisposableExtensionsAreDisposedWithContainerButNotRemoved()
        {
            DisposableExtension extension = new DisposableExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Dispose();

            Assert.True(extension.Disposed);
            Assert.False(extension.Removed);
        }

        [Fact]
        public void OnlyDisposableExtensionAreDisposed()
        {
            DisposableExtension extension = new DisposableExtension();
            NoopExtension noop = new NoopExtension();

            IUnityContainer container = new UnityContainer()
                .AddExtension(noop)
                .AddExtension(extension);

            container.Dispose();

            Assert.True(extension.Disposed);
        }

        [Fact]
        public void CanSafelyDisposeContainerTwice()
        {
            DisposableExtension extension = new DisposableExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.Dispose();
            container.Dispose();
        }

        [Fact]
        public void RemovedExtensionsAreDisposed()
        {
            DisposableExtension extension = new DisposableExtension();
            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            container.RemoveAllExtensions();

            Assert.True(extension.Removed);
            Assert.True(extension.Disposed);
        }

        private class DisposableExtension : UnityContainerExtension, IDisposable
        {
            public bool Disposed = false;
            public bool Removed = false;

            protected override void Initialize()
            {
            }

            public override void Remove()
            {
                this.Removed = true;
            }

            public void Dispose()
            {
                if (this.Disposed)
                {
                    throw new Exception("Can't dispose twice!");
                }
                this.Disposed = true;
            }
        }

        private class NoopExtension : UnityContainerExtension
        {
            protected override void Initialize()
            {
            }
        }
    }
}
