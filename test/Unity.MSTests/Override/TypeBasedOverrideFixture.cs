// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

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
    /// Summary description for TypeBasedOverrideFixture
    /// </summary>
    [TestClass]
    public class TypeBasedOverrideFixture
    {
        public TypeBasedOverrideFixture()
        {
        }

        [TestMethod]
        public void TypeBasedOverrideWithConstructorExactTypeMatch()
        {
            TypeToInject2ForTypeOverride defaultValue = new TypeToInject2ForTypeOverride(111);
            TypeToInject2ForTypeOverride overrideValue = new TypeToInject2ForTypeOverride(222);
            ParameterOverride overrideParam = new ParameterOverride("injectedObject", overrideValue);
            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(TypeToToUndergoeTypeBasedInject2), overrideParam);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<IForToUndergoeInject, TypeToToUndergoeTypeBasedInject2>(new InjectionConstructor(defaultValue));
            var result = container.Resolve<IForToUndergoeInject>(overrideDecorator);

            Assert.AreEqual<int>(222, result.IForTypeToInject.Value);
        }

        [TestMethod]
        public void TypeBasedOverrideWithBuildUp()
        {
            MySimpleType instance = new MySimpleType();
            instance.X = 111;

            PropertyOverride overrideParam = new PropertyOverride("X", 222);
            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(MySimpleType), overrideParam);

            UnityContainer container = new UnityContainer();

            var result = container.BuildUp<MySimpleType>(instance, overrideDecorator);
            Assert.AreEqual<int>(222, result.X);
        }

        [TestMethod]
        public void TypeBasedOverrideInjectsDependentTypeProperty()
        {
            ParameterOverride overrideParam = new ParameterOverride("value", 222);
            PropertyOverride overrideProp = new PropertyOverride("PropertyToInject", "TestOverrideProp");
            TypeBasedOverride typeOverrideConstructor = new TypeBasedOverride(typeof(TypeToInject3ForTypeOverride), overrideParam);
            TypeBasedOverride typeOverrideProp = new TypeBasedOverride(typeof(TypeToInject3ForTypeOverride), overrideProp);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<TypeToUndergoeTypeBasedInject1>().RegisterType<IForTypeToInject, TypeToInject3ForTypeOverride>(new InjectionConstructor(111), new InjectionProperty("PropertyToInject", "DefaultValue"));
            var result = container.Resolve<TypeToUndergoeTypeBasedInject1>(typeOverrideConstructor, typeOverrideProp);
            TypeToInject3ForTypeOverride overriddenProperty = (TypeToInject3ForTypeOverride)result.IForTypeToInject;

            Assert.AreEqual<int>(222, overriddenProperty.Value);
            Assert.AreEqual<string>("TestOverrideProp", overriddenProperty.PropertyToInject);
        }

        [TestMethod]
        public void WhenResolvingAnOpenGenericType()
        {
            var container = new UnityContainer();

            try
            {
                container.Resolve(typeof(List<>));
            }
            catch (ResolutionFailedException ex)
            {
                Assert.AreEqual(typeof(ArgumentException), ex.InnerException.GetType());
            }
        }

        [TestMethod]
        public void WhenTryingToResolveAPrimitiveType()
        {
            Type[] primitive = new Type[]
            {
                typeof(sbyte),
                typeof(byte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(char),
                typeof(float),
                typeof(double),
                typeof(bool),
                typeof(decimal),
                typeof(string)
            };

            var container = new UnityContainer();

            foreach (Type t in primitive)
            {
                try
                {
                    container.Resolve(t);
                    Assert.Fail("Cannot resolve a primitive type");
                }
                catch (ResolutionFailedException ex)
                {
                    Assert.AreEqual(typeof(InvalidOperationException), ex.InnerException.GetType());
                }
            }
        }

        [TestMethod]
        public void TypeBasedOverrideCollectionInCompositeOverrideInjectionTest()
        {
            ParameterOverride overrideParam = new ParameterOverride("value", 222);
            PropertyOverride overrideProp = new PropertyOverride("PropertyToInject", "TestOverrideProp");
            TypeBasedOverride typeOverrideConstructor = new TypeBasedOverride(typeof(TypeToInject3ForTypeOverride), overrideParam);
            TypeBasedOverride typeOverrideProp = new TypeBasedOverride(typeof(TypeToInject3ForTypeOverride), overrideProp);
            CompositeResolverOverride compositeOverride = new CompositeResolverOverride();
            compositeOverride.Add(typeOverrideConstructor);
            compositeOverride.Add(typeOverrideProp);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<TypeToUndergoeTypeBasedInject1>().RegisterType<IForTypeToInject, TypeToInject3ForTypeOverride>(new InjectionConstructor(111), new InjectionProperty("PropertyToInject", "DefaultValue"));
            var result = container.Resolve<TypeToUndergoeTypeBasedInject1>(compositeOverride);
            TypeToInject3ForTypeOverride overriddenProperty = (TypeToInject3ForTypeOverride)result.IForTypeToInject;

            Assert.AreEqual<int>(222, overriddenProperty.Value);
            Assert.AreEqual<string>("TestOverrideProp", overriddenProperty.PropertyToInject);
        }

        [TestMethod]
        public void TypeBasedOverrideNullCheckForResolverOverride()
        {
            AssertHelper.ThrowsException<ArgumentNullException>(() => new TypeBasedOverride(typeof(TypeToInject2ForTypeOverride), null));
        }

        [TestMethod]
        public void TypeBasedOverrideInjectsDependentTypeConstructor()
        {
            ParameterOverride overrideParam = new ParameterOverride("value", 222);
            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(TypeToInject2ForTypeOverride), overrideParam);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<TypeToToUndergoeTypeBasedInject2>().RegisterType<TypeToInject2ForTypeOverride>(new InjectionConstructor(111));
            var result = container.Resolve<TypeToToUndergoeTypeBasedInject2>(overrideDecorator);

            Assert.AreEqual<int>(222, result.IForTypeToInject.Value);
        }

        [TestMethod]
        public void TypeBasedOverrideWithResolveAll()
        {
            IForTypeToInject defaultValue = new TypeToInject1ForTypeOverride(111);
            IForTypeToInject overrideValue = new TypeToInject1ForTypeOverride(222);
            ParameterOverride overrideParam = new ParameterOverride("injectedObject", overrideValue);
            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(TypeToUndergoeTypeBasedInject1), overrideParam);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<IForToUndergoeInject, TypeToUndergoeTypeBasedInject1>(new InjectionConstructor(defaultValue)).RegisterType<IForToUndergoeInject, TypeToUndergoeTypeBasedInject1>("Named", new InjectionConstructor(defaultValue));
            var resultList = container.ResolveAll<IForToUndergoeInject>(overrideDecorator);

            foreach (var result in resultList)
            {
                Assert.AreEqual<int>(222, result.IForTypeToInject.Value);
            }
        }

        [TestMethod]
        public void TypeBasedOverrideConstructorWithNoTypeMatch()
        {
            IForTypeToInject defaultValue = new TypeToInject1ForTypeOverride(111);
            IForTypeToInject overrideValue = new TypeToInject2ForTypeOverride(222);
            ParameterOverride overrideParam = new ParameterOverride("injectedObject", overrideValue);
            TypeBasedOverride overrideDecorator = new TypeBasedOverride(typeof(int), overrideParam);

            IUnityContainer container = new UnityContainer();

            container.RegisterType<IForToUndergoeInject, TypeToUndergoeTypeBasedInject1>(new InjectionConstructor(defaultValue));
            var result = container.Resolve<IForToUndergoeInject>(overrideDecorator);

            Assert.AreEqual<int>(111, result.IForTypeToInject.Value);
        }

        [TestMethod]
        public void TypeBasedOverrideConstructorWithNullOverride()
        {
            TypeToInject2ForTypeOverride defaultValue = new TypeToInject2ForTypeOverride(111);
            TypeToInject2ForTypeOverride overrideValue = null;
            AssertHelper.ThrowsException<ArgumentNullException>(() => new ParameterOverride("injectedObject", overrideValue));
        }
    }
}