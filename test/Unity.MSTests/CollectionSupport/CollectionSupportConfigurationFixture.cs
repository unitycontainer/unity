// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Unity.Tests.CollectionSupport
{
#pragma warning disable CS0618 // Type or member is obsolete
    [TestClass]
    public class CollectionSupportConfigurationFixture
    {
        [TestMethod]
        public void ConfiguringPropertyInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            // RL: Change from obsolete to newer method?
            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionProperty("ArrayProperty", new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringPropertyInjectionYieldsProperlyInjectedObject_Empty()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionProperty("ArrayProperty", new ResolvedArrayParameter<CollectionSupportTestClass>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringPropertyInjectionYieldsProperlyInjectedObject_Specified()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionProperty("ArrayProperty", new ResolvedArrayParameter<CollectionSupportTestClass>(new CollectionSupportTestClass())));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringPropertyInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionProperty("ArrayProperty", new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringGenericPropertyInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionProperty("ArrayProperty", new GenericResolvedArrayParameter("T")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringGenericPropertyInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionProperty("ArrayProperty", new GenericParameter("T[]")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayProperty.Length);
        }

        [TestMethod]
        public void ConfiguringCtorInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionConstructor(new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringCtorInjectionYieldsProperlyInjectedObject_Empty()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionConstructor(new ResolvedArrayParameter<CollectionSupportTestClass>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringCtorInjectionYieldsProperlyInjectedObject_Specified()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionConstructor(new ResolvedArrayParameter<CollectionSupportTestClass>(new CollectionSupportTestClass())));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringCtorInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionConstructor(new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringGenericCtorInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionConstructor(new GenericParameter("T[]")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(0, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringGenericCtorInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionConstructor(new GenericParameter("T[]")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.IsNull(resolved.ArrayMethod);
            Assert.AreEqual(1, resolved.ArrayCtor.Length);
        }

        [TestMethod]
        public void ConfiguringMethodInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionMethod("InjectionMethod", new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(0, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        public void ConfiguringMethodInjectionYieldsProperlyInjectedObject_Empty()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionMethod("InjectionMethod", new ResolvedArrayParameter<CollectionSupportTestClass>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(0, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        public void ConfiguringMethodInjectionYieldsProperlyInjectedObject_Specified()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionMethod("InjectionMethod", new ResolvedArrayParameter<CollectionSupportTestClass>(new CollectionSupportTestClass())));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(1, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        public void ConfiguringMethodInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor<ConfigurationCollectionSupportTestClass>(
                    new InjectionMethod("InjectionMethod", new ResolvedParameter<CollectionSupportTestClass[]>()));

            ConfigurationCollectionSupportTestClass resolved = container.Resolve<ConfigurationCollectionSupportTestClass>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(1, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        public void ConfiguringGenericMethodInjectionYieldsProperlyInjectedObject()
        {
            IUnityContainer container = new UnityContainer();

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionMethod("InjectionMethod", new GenericResolvedArrayParameter("T")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(0, resolved.ArrayMethod.Length);
        }

        [TestMethod]
        public void ConfiguringGenericMethodInjectionYieldsProperlyInjectedObject_Elements()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<CollectionSupportTestClass>("element", new ContainerControlledLifetimeManager());

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConfigurationCollectionSupportTestClassGeneric<>),
                    new InjectionMethod("InjectionMethod", new GenericParameter("T[]")));

            ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass> resolved = container.Resolve<ConfigurationCollectionSupportTestClassGeneric<CollectionSupportTestClass>>();

            Assert.IsNotNull(resolved);
            Assert.IsNull(resolved.ArrayCtor);
            Assert.IsNull(resolved.ArrayProperty);
            Assert.AreEqual(1, resolved.ArrayMethod.Length);
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
