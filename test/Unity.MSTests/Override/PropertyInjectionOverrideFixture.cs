// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Threading;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.Override
{
    /// <summary>
    /// Summary description for PropertyInjectionOverrideFixture
    /// </summary>
    [TestClass]
    public class PropertyInjectionOverrideFixture
    {
        [TestMethod]
        public void CanOverrideInjectionProperty()
        {
            TypeToInjectForPropertyOverride1 defaultObject = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideObject = new TypeToInjectForPropertyOverride2(222);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>(new InjectionProperty("InjectedObject",
                defaultObject));
            var result1 =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideObject));
            var result2 = container.Resolve<SubjectType1ToInjectForPropertyOverride>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
        }

        [TestMethod]
        public void CanOverrideInjectionPropertyWithNull()
        {
            TypeToInjectForPropertyOverride1 defaultObject = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideObject = null;

            IUnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>(new InjectionProperty("InjectedObject",
                defaultObject));

            AssertHelper.ThrowsException<ArgumentNullException>(
                () =>
                    container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                        overrideObject)));
        }

        [TestMethod]
        public void CanOverrideMultipleInjectionProperties()
        {
            TypeToInjectForPropertyOverride1 defaultObject = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideObject = new TypeToInjectForPropertyOverride2(222);
            int defaultValue = 111;
            int overrideValue = 222;

            IUnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>(
                new InjectionProperty("InjectedObject", defaultObject), new InjectionProperty("X", defaultValue));
            var result1 =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(
                    new PropertyOverride("InjectedObject", overrideObject), new PropertyOverride("X", overrideValue));
            var result2 = container.Resolve<SubjectType1ToInjectForPropertyOverride>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.AreEqual<int>(result1.X, overrideValue);
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
            Assert.AreEqual<int>(result2.X, defaultValue);
        }

        [TestMethod]
        public void CanOverrideMultipleInjectionPropertiesWithOverrideCollection()
        {
            TypeToInjectForPropertyOverride1 defaultObject = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideObject = new TypeToInjectForPropertyOverride2(222);
            int defaultValue = 111;
            int overrideValue = 222;
            PropertyOverrides overrides = new PropertyOverrides();
            overrides.Add("X", overrideValue);
            overrides.Add("InjectedObject", overrideObject);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>(
                new InjectionProperty("InjectedObject", defaultObject), new InjectionProperty("X", defaultValue));
            var result1 =
                (SubjectType1ToInjectForPropertyOverride)
                    container.Resolve(typeof(SubjectType1ToInjectForPropertyOverride), overrides);
            var result2 = container.Resolve<SubjectType1ToInjectForPropertyOverride>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.AreEqual<int>(result1.X, overrideValue);
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
            Assert.AreEqual<int>(result2.X, defaultValue);
        }

        [TestMethod]
        public void CanOverrideInjectionPropertyMultipleCalls()
        {
            TypeToInjectForPropertyOverride1 defaultObject = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideObject1 = new TypeToInjectForPropertyOverride2(222);
            TypeToInjectForPropertyOverride3 overrideObject2 = new TypeToInjectForPropertyOverride3(333);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>(new InjectionProperty("InjectedObject",
                defaultObject));
            var result1 =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideObject1));
            var result2 =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideObject2));
            var result3 = container.Resolve<SubjectType1ToInjectForPropertyOverride>();

            Assert.IsInstanceOfType(result1.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride3));
            Assert.IsInstanceOfType(result3.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
        }

        [TestMethod]
        public void PropertyOverrideWithBuildUp()
        {
            MySimpleTypeForPropertyOverride instance = new MySimpleTypeForPropertyOverride();
            instance.X = 111;

            PropertyOverride overrideParam = new PropertyOverride("X", 222);

            IUnityContainer container = new UnityContainer();

            var result = container.BuildUp<MySimpleTypeForPropertyOverride>(instance, overrideParam);

            Assert.AreEqual<int>(222, result.X);
        }

        [TestMethod]
        public void CanOverrideInjectionPropertyForPolymorphicTypesInCollection()
        {
            TypeToInjectForPropertyOverride1 defaultInjected = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideInjected = new TypeToInjectForPropertyOverride2(222);

            UnityContainer container = new UnityContainer();

            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType1ToInjectForPropertyOverride>(
                "Test1", new InjectionProperty("InjectedObject", defaultInjected));
            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType2ToInjectForPropertyOverride>(
                "Test2", new InjectionProperty("InjectedObject", defaultInjected));
            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType3ToInjectForPropertyOverride>(
                "Test3", new InjectionProperty("InjectedObject", defaultInjected));

            var resultList =
                container.ResolveAll<ISubjectTypeToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideInjected));

            int count = 0;
            foreach (var result in resultList)
            {
                count++;
                Assert.IsInstanceOfType(result.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            }

            Assert.AreEqual<int>(3, count);
        }

        [TestMethod]
        public void CannotOverrideInjectionPropertyForContainerControlledInstanceUsingResolve()
        {
            TypeToInjectForPropertyOverride1 defaultInjected = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideInjected = new TypeToInjectForPropertyOverride2(222);

            UnityContainer container = new UnityContainer();

            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType1ToInjectForPropertyOverride>(
                new ContainerControlledLifetimeManager(), new InjectionProperty("InjectedObject", defaultInjected));

            var result1 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>();
            var result2 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>(new PropertyOverride("InjectedObject", overrideInjected));

            Assert.AreEqual(result1, result2);
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
            Assert.AreEqual<int>(111, result2.InjectedObject.Value);
        }

        [TestMethod]
        public void CanOverrideInjectionPropertyForContainerControlledInstanceUsingBuildUp()
        {
            TypeToInjectForPropertyOverride1 defaultInjected = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideInjected = new TypeToInjectForPropertyOverride2(222);

            UnityContainer container = new UnityContainer();

            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType1ToInjectForPropertyOverride>(
                new ContainerControlledLifetimeManager(), new InjectionProperty("InjectedObject", defaultInjected));

            var result1 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>();
            var result2 = container.BuildUp(result1, new PropertyOverride("InjectedObject", overrideInjected));
            var result3 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>();

            Assert.AreEqual<ISubjectTypeToInjectForPropertyOverride>(result1, result2);
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.AreEqual<int>(222, result2.InjectedObject.Value);
            Assert.IsInstanceOfType(result3.InjectedObject, typeof(TypeToInjectForPropertyOverride2));
            Assert.AreEqual<int>(222, result3.InjectedObject.Value);
        }

        [TestMethod]
        public void TestPropertyOverridesWithExternallyControlledLifeTimeManager()
        {
            TypeToInjectForPropertyOverride1 defaultInjected = new TypeToInjectForPropertyOverride1(111);
            TypeToInjectForPropertyOverride2 overrideInjected = new TypeToInjectForPropertyOverride2(222);
            TypeToInjectForPropertyOverride3 overrideInjected1 = new TypeToInjectForPropertyOverride3(333);

            UnityContainer container = new UnityContainer();

            container.RegisterType<ISubjectTypeToInjectForPropertyOverride, SubjectType1ToInjectForPropertyOverride>(
                new ExternallyControlledLifetimeManager(), new InjectionProperty("InjectedObject", defaultInjected));

            var result1 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>();
            var result2 = container.Resolve<ISubjectTypeToInjectForPropertyOverride>(new PropertyOverride("InjectedObject", overrideInjected));

            Assert.AreEqual(result1, result2);
            Assert.IsInstanceOfType(result2.InjectedObject, typeof(TypeToInjectForPropertyOverride1));

            result1 = null;
            result2 = null;
            System.GC.Collect();

            var result3 =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideInjected1));

            Assert.IsInstanceOfType(result3.InjectedObject, typeof(TypeToInjectForPropertyOverride3));
        }

        [TestMethod]
        public void CanOverridePropertyWithoutDefaultWithDependencyAttribute()
        {
            TypeToInjectForPropertyOverride1 overrideObject = new TypeToInjectForPropertyOverride1(222);

            UnityContainer container = new UnityContainer();

            container.RegisterType<SubjectType1ToInjectForPropertyOverride>();
            var result =
                container.Resolve<SubjectType1ToInjectForPropertyOverride>(new PropertyOverride("InjectedObject",
                    overrideObject));

            Assert.IsInstanceOfType(result.InjectedObject, typeof(TypeToInjectForPropertyOverride1));
        }

        [TestMethod]
        public void PropertyOverrideWithCode()
        {
            TestTypeInConfig overrideObject = new TestTypeInConfig();

            IUnityContainer container = new UnityContainer();

            container.RegisterType<TestTypeInConfig>(new InjectionProperty("X", -111),
                new InjectionProperty("Y", "default"), new InjectionConstructor());
            var defaultResult = container.Resolve<TestTypeInConfig>();
            PropertyOverrides overrides = new PropertyOverrides();
            overrides.Add("X", 9999);
            overrides.Add("Y", "Overridden");
            var overriddenResult = container.Resolve<TestTypeInConfig>(overrides);

            Assert.AreEqual<int>(-111, defaultResult.X);
            Assert.AreEqual<string>("default", defaultResult.Y);
            Assert.AreEqual<int>(9999, overriddenResult.X);
            Assert.AreEqual<string>("Overridden", overriddenResult.Y);
        }
    }
}