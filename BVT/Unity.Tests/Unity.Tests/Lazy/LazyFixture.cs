// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.Practices.Unity;
using Unity.Tests.TestObjects;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.Lazy
{
    [TestClass]
    public class LazyFixture
    {
        [TestMethod]
        public void ResolveLazyHavingDependencies()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IService, EmailService>();
            container.RegisterType<IBase, Base>();
            var lazy = container.Resolve<Lazy<IBase>>();
            
            Assert.IsFalse(lazy.IsValueCreated);
            
            var value = lazy.Value;
            
            Assert.IsNotNull(value.Service);
        }

        [TestMethod]
        public void ResolveLazyHavingLazyDependencies()
        {
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<Lazy<EmailService>>();
            //container.RegisterType<IService, EmailService>();
            container.RegisterType<ILazyDependency, LazyDependency>();
            var lazy = container.Resolve<Lazy<ILazyDependency>>();

            Assert.IsFalse(lazy.IsValueCreated);
            
            var ld = (LazyDependency)lazy.Value;
            
            Assert.IsFalse(ld.Service.IsValueCreated);
            Assert.IsNotNull(ld.Service.Value);
        }

        [TestMethod]
        public void RegisterLazyInstanceAndResolve()
        {
            IUnityContainer container = new UnityContainer();
            
            var lazy = new Lazy<EmailService>();
            container.RegisterInstance(lazy);
            //container.RegisterType<IService, EmailService>();
            var lazy1 = container.Resolve<Lazy<EmailService>>();
            var lazy3 = container.Resolve<Lazy<EmailService>>();
            
            Assert.IsTrue(lazy == lazy1);
            Assert.IsTrue(lazy == lazy3);
            Assert.IsFalse(lazy.IsValueCreated);
        }

        [TestMethod]
        public void BuildupLazyInstance()
        {
            IUnityContainer container = new UnityContainer();
        
            container.RegisterType<Lazy<EmailService>>();
            var lazyDependency = new Lazy<LazyDependency>();
            var lazy = container.BuildUp(lazyDependency);
            var lazyreturned = container.Resolve<Lazy<LazyDependency>>();
            
            Assert.AreEqual(lazy, lazyDependency);
            Assert.IsFalse(lazyreturned.IsValueCreated);
            
            var ld = (LazyDependency)lazyreturned.Value;
            
            Assert.IsFalse(ld.Service.IsValueCreated);
            Assert.IsNotNull(ld.Service.Value);
        }

        [TestMethod]
        public void InjectToNonDefaultConstructorWithLazy()
        {
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<Lazy<EmailService>>();
            container.RegisterType<Lazy<LazyDependency>>();
            var resolved = container.Resolve<Lazy<LazyDependency>>();
            
            Assert.IsFalse(resolved.IsValueCreated);
            
            var lazy = resolved.Value;

            Assert.IsFalse(resolved.Value.Service.IsValueCreated);
            Assert.IsNotNull(resolved.Value.Service.Value);
        }
    }
}