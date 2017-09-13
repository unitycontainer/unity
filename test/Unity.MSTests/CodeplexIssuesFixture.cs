// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Unity.Tests.Generics;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for Codeplexbugs
    /// </summary>
    [TestClass]
    public class CodeplexIssuesFixture
    {
        [TestMethod]
        //http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=2618
        public void BUG2883_CanResolveTypesWithOutParametersInCtor()
        {
            var unity = new UnityContainer();

            int count = 0;
            var test = new TestClassWithOutParametersInCtor(out count);

            unity.BuildUp(test);

            Assert.AreEqual(count, 5);
        }

        /// <summary>
        /// Make sure you can resolve a timespan
        /// </summary>
        /// <remarks>Bug 3982</remarks>
        [TestMethod]
        public void CanResolveTimeSpanAfterRegisterType()
        {
            var unity = new UnityContainer();

            unity.RegisterType<TimeSpan>(new InjectionConstructor((long)0));

            TimeSpan? ts = unity.Resolve<TimeSpan>();

            Assert.IsTrue(ts.HasValue);
        }

        /// <summary>
        /// Make sure you can resolve a timespan
        /// </summary>
        /// <remarks>Bug 3982</remarks>
        [TestMethod]
        public void CanResolveTimeSpanAfterRegisterInstance()
        {
            var unity = new UnityContainer();

            unity.RegisterInstance<TimeSpan>(new TimeSpan(), new ExternallyControlledLifetimeManager());

            TimeSpan? ts = unity.Resolve<TimeSpan>();

            Assert.IsTrue(ts.HasValue);
        }

        public class TestClassWithOutParametersInCtor
        {
            public TestClassWithOutParametersInCtor(out int count)
            {
                count = 5;
            }
        }

        [TestMethod]
        //http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=2618
        public void BUG2883_CanResolveTypesWithRefParametersInCtor()
        {
            var unity = new UnityContainer();

            int count = 0;
            var test = new TestClassWithRefParametersInCtor(ref count);

            unity.BuildUp(test);

            Assert.AreEqual(count, 5);
        }

        public class TestClassWithRefParametersInCtor
        {
            public TestClassWithRefParametersInCtor(ref int count)
            {
                count = 5;
            }
        }

        [TestMethod]
        public void Bug17153_ChildContainerOverridesParentRegistration()
        {
            //RegisterType in parent container overrides RegisterInstance in child container
            IUnityContainer parent = new UnityContainer()
                .RegisterType<IFoo, RealFoo>();

            IUnityContainer child = parent.CreateChildContainer()
                .RegisterInstance<IFoo>(new MockFoo());
            
            IFoo result = child.Resolve<IFoo>();

            Assert.IsInstanceOfType(result, typeof(MockFoo));
        }

        // https://www.codeplex.com/Thread/View.aspx?ProjectName=unity&ThreadId=25301
        [TestMethod]
        public void Bug_16900_CanUseNonDefaultLifetimeManagerWithOpenGenericRegistration()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType(typeof(ITestFoo<>),
                typeof(MyFoo<>),
                new ContainerControlledLifetimeManager());
            
            ITestFoo<int> intFoo = container.Resolve<ITestFoo<int>>();
            ITestFoo<string> stringFoo1 = container.Resolve<ITestFoo<string>>();
            ITestFoo<string> stringFoo2 = container.Resolve<ITestFoo<string>>();

            Assert.AreSame(stringFoo1, stringFoo2);
        }

        // https://www.codeplex.com/Thread/View.aspx?ProjectName=unity&ThreadId=25301
        [TestMethod]
        public void Bug_16900_CanOverrideGenericLifetimeManagerWithSpecificOne()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ITestFoo<>),
                    typeof(MyFoo<>),
                    new ContainerControlledLifetimeManager())
                .RegisterType(typeof(MyFoo<double>), new TransientLifetimeManager());

            ITestFoo<string> string1 = container.Resolve<ITestFoo<string>>();
            ITestFoo<string> string2 = container.Resolve<ITestFoo<string>>();

            ITestFoo<double> double1 = container.Resolve<ITestFoo<double>>();
            ITestFoo<double> double2 = container.Resolve<ITestFoo<double>>();

            Assert.AreSame(string1, string2);
            Assert.AreNotSame(double1, double2);
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee on the ctor.
        /// </summary>
        [TestMethod]
        public void Bug_18117_NoDecorator()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(Repository<,>));

            IRepository<string, int> repository;
            repository = uc.Resolve<IRepository<string, int>>();

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(Repository<string, int>));
            Assert.IsNull(repository.Get(5));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee on the ctor.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorCTOR()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(Repository<,>));
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingCTORRepository<,>), "Logging");

            IRepository<string, int> repository;

            repository = uc.Resolve<IRepository<string, int>>();

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(Repository<string, int>));
            Assert.IsNull(repository.Get(5));

            repository = uc.Resolve<IRepository<string, int>>("Logging");

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(LoggingCTORRepository<string, int>));
            Assert.IsNull(repository.Get(5));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee with property injection.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorProperty()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(Repository<,>));
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingPropertyRepository<,>), "Logging");

            IRepository<string, int> repository;

            repository = uc.Resolve<IRepository<string, int>>();

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(Repository<string, int>));
            Assert.IsNull(repository.Get(5));

            repository = uc.Resolve<IRepository<string, int>>("Logging");

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(LoggingPropertyRepository<string, int>));
            Assert.IsNull(repository.Get(5));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee with method injection.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorMethod()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(Repository<,>));
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingMethodRepository<,>), "Logging");

            IRepository<string, int> repository;

            repository = uc.Resolve<IRepository<string, int>>();

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(Repository<string, int>));
            Assert.IsNull(repository.Get(5));

            repository = uc.Resolve<IRepository<string, int>>("Logging");

            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(LoggingMethodRepository<string, int>));
            Assert.IsNull(repository.Get(5));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee on the ctor.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorCTORDecorateeNotRegistered()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingCTORRepository<,>), "Logging");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<IRepository<string, int>>("Logging"));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee with property injection.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorPropertyDecorateeNotRegistered()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingPropertyRepository<,>), "Logging");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<IRepository<string, int>>("Logging"));
        }

        /// <summary>
        /// Verifies that Generic Decorator Chains work correctly...
        /// 
        /// i.e. resolving a IFoo`2, that is decorated by a decorator that receives the decoratee by method injection.
        /// </summary>
        [TestMethod]
        public void Bug_18117_WithDecoratorMethodDecorateeNotRegistered()
        {
            UnityContainer uc = new UnityContainer();

            //cannot use RegisterType<,> because of the TTo : TFrom constraint
            uc.RegisterType(typeof(IRepository<,>), typeof(LoggingMethodRepository<,>), "Logging");

            AssertHelper.ThrowsException<ResolutionFailedException>(() => uc.Resolve<IRepository<string, int>>("Logging"));
        }

        #region Bug5112

        // code plex issue http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=6999
        public class OpenGenericSample<TOne, TTwo> : IOpenGenericSample<TOne, TTwo>
        {
        }

        public interface IOpenGenericSample<TOne, TTwo>
        {
        }

        [TestMethod]
        public void Given_an_open_generic_registration_with_a_container_controlled_lifetime_then_resolving_twice_should_return_the_same_instance()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType(typeof(IOpenGenericSample<,>), typeof(OpenGenericSample<,>), new ContainerControlledLifetimeManager());

            var sampleOne = container.Resolve<IOpenGenericSample<string, IEnumerable<int>>>();
            var sampleTwo = container.Resolve<IOpenGenericSample<string, IEnumerable<int>>>();

            Assert.AreSame(sampleOne, sampleTwo);
            Assert.AreEqual(sampleOne.GetHashCode(), sampleTwo.GetHashCode());
        }

        [TestMethod]
        public void Given_an_open_generic_registration_with_a_container_controlled_lifetime_then_resolving_twice_on_different_children_containers_should_return_the_same_instance()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType(typeof(IOpenGenericSample<,>), typeof(OpenGenericSample<,>), new ContainerControlledLifetimeManager());

            var sampleOne = container.CreateChildContainer().Resolve<IOpenGenericSample<int, int>>();
            var sampleTwo = container.CreateChildContainer().Resolve<IOpenGenericSample<int, int>>();

            Assert.AreSame(sampleOne, sampleTwo);
            Assert.AreEqual(sampleOne.GetHashCode(), sampleTwo.GetHashCode());
        }

        #endregion

        public interface IRepository<TEntity, TIDEntity>
        {
            TEntity Get(TIDEntity id);
        }

        public class Repository<TEntity, TIdEntity> : IRepository<TEntity, TIdEntity>
        {
            public TEntity Get(TIdEntity id)
            {
                return default(TEntity);
            }
        }

        public class LoggingCTORRepository<TEntity, TIdEntity> : IRepository<TEntity, TIdEntity>
        {
            private IRepository<TEntity, TIdEntity> decoratee;

            public LoggingCTORRepository(IRepository<TEntity, TIdEntity> decoratee)
            {
                this.decoratee = decoratee;
            }

            public TEntity Get(TIdEntity id)
            {
                Assert.IsNotNull(decoratee);

                //log before
                TEntity entity = decoratee.Get(id);
                //log after

                return entity;
            }
        }

        public class LoggingPropertyRepository<TEntity, TIdEntity> : IRepository<TEntity, TIdEntity>
        {
            private IRepository<TEntity, TIdEntity> decoratee;

            [Dependency]
            public IRepository<TEntity, TIdEntity> Decoratee
            {
                get { return decoratee; }
                set { decoratee = value; }
            }

            public TEntity Get(TIdEntity id)
            {
                Assert.IsNotNull(decoratee);

                //log before
                TEntity entity = decoratee.Get(id);
                //log after

                return entity;
            }
        }

        public class LoggingMethodRepository<TEntity, TIdEntity> : IRepository<TEntity, TIdEntity>
        {
            private IRepository<TEntity, TIdEntity> decoratee;

            [InjectionMethod]
            public void SetDecoratee(IRepository<TEntity, TIdEntity> decoratee)
            {
                this.decoratee = decoratee;
            }

            public TEntity Get(TIdEntity id)
            {
                Assert.IsNotNull(decoratee);

                //log before
                TEntity entity = decoratee.Get(id);
                //log after

                return entity;
            }
        }
    }

    public interface ISomeInterface
    {
        int Count();
    }

    internal class SomeClass : ISomeInterface
    {
        private string[] values = null;

        public SomeClass()
        {
            this.values = new string[1];
        }

        public SomeClass(string[] values)
        {
            this.values = values;
        }

        public int Count()
        {
            return null == values ? 0 : values.Length;
        }
    }

    public interface IFoo1<T>
    {
    }

    public class MyFoo1<T> : IFoo1<T>
    {
    }

    public class MyStringFoo1 : IFoo1<string>
    {
    }
    public interface ITestFoo<T>
    {
    }

    public class MyFoo<T> : ITestFoo<T>
    {
    }

    public class RealFoo : IFoo
    {
    }

    public class MockFoo : IFoo
    {
    }

    public class TestCodePlexBug
    {
        private bool defaultConstructorCalled;
        private int myInt;
        private string myString;

        public TestCodePlexBug()
        {
            defaultConstructorCalled = true;
        }

        public TestCodePlexBug(int someNumber)
        {
            myInt = someNumber;
        }

        public TestCodePlexBug(string someString)
        {
            myString = someString;
        }

        public int MyInt
        {
            get { return myInt; }
        }

        public string MyString
        {
            get { return myString; }
        }

        public bool DefaultConstructorCalled
        {
            get { return defaultConstructorCalled; }
        }
    }
}