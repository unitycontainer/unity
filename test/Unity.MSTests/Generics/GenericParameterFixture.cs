// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


namespace Unity.Tests.Generics
{
#pragma warning disable CS0618 // Type or member is obsolete
    [TestClass]
    public class GenericParameterFixture
    {
        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_ByDefaultUsesGreediestGenericCtor()
        {
            UnityContainer uc = new UnityContainer();

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.PropT1);
            Assert.IsNotNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSelectCtorToUseWhenThereIsAmbiguity_T1()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(new GenericParameter("T1")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSelectCtorToUseWhenThereIsAmbiguity_T2()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(new GenericParameter("T2")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNull(o.PropT1);
            Assert.IsNotNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSetDependencyProperty()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionProperty("PropT3", new GenericParameter("T3")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNotNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }


        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSetDependencyPropertyWithoutExplicitValue()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionProperty("PropT3"));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNotNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSelectOverloadMethodWhenThereIsAmbiguity_T1()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("Set", new GenericParameter("T1")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSelect2OverloadMethods()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("Set", new GenericParameter("T1")),
                    new InjectionMethod("Set", new GenericParameter("T4")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNotNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNotNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanSelectOverloadMethodWhenThereIsAmbiguity_T2()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("Set", new GenericParameter("T2")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNull(o.PropT1);
            Assert.IsNotNull(o.PropT2);
            Assert.IsNull(o.PropT3);
            Assert.IsNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanInjectMethodWithArbitraryGenerics()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("SetMultiple", new GenericParameter("T4"), new GenericParameter("T3")));

            HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD> o;
            o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericB, GenericC, GenericD>>();

            Assert.IsNotNull(o);
            Assert.IsNull(o.PropT1);
            Assert.IsNull(o.PropT2);
            Assert.IsNotNull(o.PropT3);
            Assert.IsNotNull(o.PropT4);
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        [TestMethod]
        public void Bug_18117_GenericParameter_CanInjectMethodWithArbitraryGenericsInvalidFailsAtConfiguration()
        {
            UnityContainer uc = new UnityContainer();

            AssertHelper.ThrowsException<InvalidOperationException>(
                () => uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("SetMultiple", new GenericParameter("T3"), new GenericParameter("T4"))));
        }

        /// <summary>
        /// Tests configuring injection for properties, ctors, methods that have generic types.
        /// 
        /// Formally tests GenericParameter
        /// </summary>
        /// <remarks>
        /// This test is meant to verify consistency with the way the compiler would handle the following code and compile time:
        /// <code>
        /// HaveManyGenericTypes&lt;GenericA, GenericA, GenericA, GenericA&gt; o;
        /// o = new HaveManyGenericTypes&lt;GenericA, GenericA, GenericA, GenericA&gt;();
        /// o.Set(new GenericA());
        /// </code>
        /// </remarks>
        [TestMethod]
        public void Bug_18117_GenericParameter_ClosedTypeThatCreatesAmbiguityOnMethodOverloadingFailsLikeCompilerDoes()
        {
            UnityContainer uc = new UnityContainer();

            uc.Configure<InjectedMembers>()
                .ConfigureInjectionFor(
                    typeof(HaveManyGenericTypes<,,,>),
                    new InjectionConstructor(),
                    new InjectionMethod("Set", new GenericParameter("T2")));

            HaveManyGenericTypes<GenericA, GenericA, GenericA, GenericA> o;
            AssertHelper.ThrowsException<ResolutionFailedException>(
                () => o = uc.Resolve<HaveManyGenericTypes<GenericA, GenericA, GenericA, GenericA>>());
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
