// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Tests for issues reported on Codeplex
    /// </summary>
    [TestClass]
    public class CodeplexIssuesFixture
    {
        // http://www.codeplex.com/unity/WorkItem/View.aspx?WorkItemId=1307
        [TestMethod]
        public void InjectionConstructorWorksIfItIsFirstConstructor()
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IBasicInterface, ClassWithDoubleConstructor>();
            IBasicInterface result = container.Resolve<IBasicInterface>();
        }

        // https://www.codeplex.com/Thread/View.aspx?ProjectName=unity&ThreadId=25301
        [TestMethod]
        public void CanUseNonDefaultLifetimeManagerWithOpenGenericRegistration()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType(typeof(ISomeInterface<>),
                typeof(MyTypeImplementingSomeInterface<>),
                new ContainerControlledLifetimeManager());
            ISomeInterface<int> intSomeInterface = container.Resolve<ISomeInterface<int>>();
            ISomeInterface<string> stringObj1 = container.Resolve<ISomeInterface<string>>();
            ISomeInterface<string> stringObj2 = container.Resolve<ISomeInterface<string>>();

            Assert.AreSame(stringObj1, stringObj2);
        }

        // https://www.codeplex.com/Thread/View.aspx?ProjectName=unity&ThreadId=25301
        [TestMethod]
        public void CanOverrideGenericLifetimeManagerWithSpecificOne()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ISomeInterface<>),
                    typeof(MyTypeImplementingSomeInterface<>),
                    new ContainerControlledLifetimeManager())
                .RegisterType(typeof(MyTypeImplementingSomeInterface<double>), new TransientLifetimeManager());

            ISomeInterface<string> string1 = container.Resolve<ISomeInterface<string>>();
            ISomeInterface<string> string2 = container.Resolve<ISomeInterface<string>>();

            ISomeInterface<double> double1 = container.Resolve<ISomeInterface<double>>();
            ISomeInterface<double> double2 = container.Resolve<ISomeInterface<double>>();

            Assert.AreSame(string1, string2);
            Assert.AreNotSame(double1, double2);
        }

        // https://www.codeplex.com/Thread/View.aspx?ProjectName=unity&ThreadId=26318
        [TestMethod]
        public void RegisteringInstanceInChildOverridesRegisterTypeInParent()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<IBasicInterface, ClassWithDoubleConstructor>(new ContainerControlledLifetimeManager());

            IUnityContainer child = container.CreateChildContainer()
                .RegisterInstance<IBasicInterface>(new MockBasic());

            IBasicInterface result = child.Resolve<IBasicInterface>();

            AssertExtensions.IsInstanceOfType(result, typeof(MockBasic));
        }

        // http://www.codeplex.com/unity/Thread/View.aspx?ThreadId=30292
        [TestMethod]
        public void CanConfigureGenericDictionaryForInjectionUsingRegisterType()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>),
                    new InjectionConstructor());

            IDictionary<string, string> result = container.Resolve<IDictionary<string, string>>();
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6431
        [TestMethod]
        public void AccessViolationExceptionOnx64()
        {
            var container1 = new UnityContainer();
            container1.RegisterType<InnerX64Class>();
            // SomeProperty is static, this should throw here
            AssertExtensions.AssertException<InvalidOperationException>(() =>
            {
                container1.RegisterType<OuterX64Class>(new InjectionProperty("SomeProperty"));
            });
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6491
        [TestMethod]
        public void CanResolveTimespan()
        {
            var container = new UnityContainer()
                .RegisterType<TimeSpan>(new ExternallyControlledLifetimeManager(),
                new InjectionConstructor(0L));
            var expected = new TimeSpan();
            var result = container.Resolve<TimeSpan>();

            Assert.AreEqual(expected, result);
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6053
        [TestMethod]
        public void ResolveAllWithChildDoesNotRepeatOverriddenRegistrations()
        {
            var parent = new UnityContainer()
                .RegisterInstance("str1", "string1")
                .RegisterInstance("str2", "string2");

            var child = parent.CreateChildContainer()
                .RegisterInstance("str2", "string20")
                .RegisterInstance("str3", "string30");

            var result = child.ResolveAll<string>();

            result.AssertContainsInAnyOrder("string1", "string20", "string30");
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6997
        [TestMethod]
        public void IsRegisteredReturnsCorrectValue()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<MyClass>(new InjectionConstructor("Name"));
            var inst = container.Resolve<MyClass>();
            Assert.IsTrue(container.IsRegistered<MyClass>());
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=3392
        [TestMethod]
        public void ResolveAllResolvesOpenGeneric()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ISomeInterface<>), typeof(MyTypeImplementingSomeInterface<>), "open")
                .RegisterType<ISomeInterface<string>, MyTypeImplementingSomeInterfaceOfString>("string");

            var results = container.ResolveAll<ISomeInterface<string>>().ToList();

            Assert.AreEqual(2, results.Count());
            results.Select(o => o.GetType())
                .AssertContainsInAnyOrder(typeof(MyTypeImplementingSomeInterface<string>), typeof(MyTypeImplementingSomeInterfaceOfString));
        }

        // http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6999
        [TestMethod]
        public void ContainerControlledOpenGenericInParentResolvesProperlyInChild()
        {
            IUnityContainer parentContainer = new UnityContainer()
                .RegisterType(typeof(ISomeInterface<>), typeof(MyTypeImplementingSomeInterface<>), new ContainerControlledLifetimeManager());

            var childOneObject = parentContainer.CreateChildContainer().Resolve<ISomeInterface<string>>();
            var childTwoObject = parentContainer.CreateChildContainer().Resolve<ISomeInterface<string>>();

            Assert.AreSame(childOneObject, childTwoObject);
        }

        // http://unity.codeplex.com/discussions/328841
        [TestMethod]
        public void MultipleResolvesAtTheSameTimeCauseConcurrencyException()
        {
            var container = new UnityContainer();
            container.RegisterInstance<string>("a value");

            const int Threads = 40;
            var barrier = new System.Threading.Barrier(Threads);
            var countdown = new CountdownEvent(Threads);
            var random = new Random();
            var errors = false;

            for (int i = 0; i < Threads; i++)
            {
                Task.Factory.StartNew(
                    wait =>
                    {
                        barrier.SignalAndWait();

                        Task.Delay((int)wait).Wait();
                        try
                        {
                            container.Resolve<ClassWithMultipleConstructorParameters>();
                        }
                        catch
                        {
                            errors = true;
                        }

                        countdown.Signal();
                    },
                    random.Next(0, 3),
                    TaskCreationOptions.LongRunning);
            }

            countdown.Wait();
            Assert.IsFalse(errors);
        }

        // https://unity.codeplex.com/workitem/11899
        [TestMethod]
        public void ResolveDelegateThrowsExplicitException()
        {
            using (var container = new UnityContainer())
            {
                try
                {
                    var func = container.Resolve<Func<string, object>>();
                }
                catch (ResolutionFailedException e)
                {
                    AssertExtensions.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                }
            }
        }

        // https://unity.codeplex.com/workitem/12745
        [TestMethod]
        public void ResolveInterfaceThrowsExplicitException()
        {
            using (var container = new UnityContainer())
            {
                try
                {
                    var func = container.Resolve<IComparable<object>>();
                }
                catch (ResolutionFailedException e)
                {
                    AssertExtensions.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                }
            }
        }

        // https://unity.codeplex.com/workitem/12745
        [TestMethod]
        public void ResolveAbstractClassThrowsExplicitException()
        {
            using (var container = new UnityContainer())
            {
                try
                {
                    var func = container.Resolve<Type>();
                }
                catch (ResolutionFailedException e)
                {
                    AssertExtensions.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                }
            }
        }

        // https://unity.codeplex.com/workitem/8777
        [TestMethod]
        public void PerResolveLifetimeIsHonoredWhenResolvingArrays()
        {
            using (var container = new UnityContainer())
            {
                container.RegisterType<ClassWithDependency>("instance1");
                container.RegisterType<ClassWithDependency>("instance2");
                container.RegisterType<ClassWithPerResolveLifetime>(new PerResolveLifetimeManager());

                var instances = container.ResolveAll<ClassWithDependency>();

                Assert.AreEqual(2, instances.Count());
                Assert.AreSame(instances.ElementAt(0).Dependency, instances.ElementAt(1).Dependency);
            }
        }

        // https://unity.codeplex.com/workitem/8777
        [TestMethod]
        public void ResolverOverridesAreUsedWhenResolvingArrayElements()
        {
            using (var container = new UnityContainer())
            {
                var overrideInstance = new ClassWithPerResolveLifetime();
                container.RegisterType<ClassWithDependency>("instance1");
                container.RegisterType<ClassWithDependency>("instance2");

                var instance =
                    container.Resolve<ClassWithDependencyOnArray>(new DependencyOverride<ClassWithPerResolveLifetime>(overrideInstance).OnType<ClassWithDependency>());

                Assert.AreEqual(2, instance.Elements.Length);
                Assert.AreSame(overrideInstance, instance.Elements[0].Dependency);
                Assert.AreSame(overrideInstance, instance.Elements[1].Dependency);
            }
        }

        public interface IBasicInterface
        {
        }

        public class ClassWithDoubleConstructor : IBasicInterface
        {
            private string myString = String.Empty;

            [InjectionConstructor]
            public ClassWithDoubleConstructor()
                : this(string.Empty)
            {
            }

            public ClassWithDoubleConstructor(string myString)
            {
                this.myString = myString;
            }
        }

        public interface ISomeInterface<T>
        {
        }

        public class MyTypeImplementingSomeInterface<T> : ISomeInterface<T>
        {
        }

        public class MyTypeImplementingSomeInterfaceOfString : ISomeInterface<string>
        {
        }

        public class MockBasic : IBasicInterface
        {
        }

        public class InnerX64Class
        {
        }

        public class OuterX64Class
        {
            public static InnerX64Class SomeProperty { get; set; }
        }

        public class MyClass
        {
            public string Name { get; set; }
            public MyClass()
            {
            }
            public MyClass(string name)
            {
                Name = name;
            }
        }

        public class ClassWithMultipleConstructorParameters
        {
            public ClassWithMultipleConstructorParameters(string parameterA, string parameterB, string parameterC, string parameterD)
            {
            }
        }

        public class ClassWithDependency
        {
            public ClassWithDependency(ClassWithPerResolveLifetime dependency)
            {
                this.Dependency = dependency;
            }

            public ClassWithPerResolveLifetime Dependency { get; private set; }
        }

        public class ClassWithPerResolveLifetime
        {
        }

        public class ClassWithDependencyOnArray
        {
            public ClassWithDependencyOnArray(ClassWithDependency[] elements)
            {
                this.Elements = elements;
            }

            public ClassWithDependency[] Elements { get; private set; }
        }
    }
}
