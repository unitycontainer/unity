// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ObjectBuilder2;
using Unity.ObjectBuilder;
using Unity.Tests.TestObjects;
using Unity.TestSupport;
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
     
    public class UnityContainerFixture
    {
        [Fact]
        public void CanCreateObjectFromUnconfiguredContainer()
        {
            IUnityContainer container = new UnityContainer();

            object o = container.Resolve<object>();

            Assert.NotNull(o);
        }

        [Fact]
        public void ContainerResolvesRecursiveConstructorDependencies()
        {
            IUnityContainer container = new UnityContainer();
            ObjectWithOneDependency dep = container.Resolve<ObjectWithOneDependency>();

            Assert.NotNull(dep);
            Assert.NotNull(dep.InnerObject);
            Assert.NotSame(dep, dep.InnerObject);
        }

        [Fact]
        public void ContainerResolvesMultipleRecursiveConstructorDependencies()
        {
            IUnityContainer container = new UnityContainer();
            ObjectWithTwoConstructorDependencies dep = container.Resolve<ObjectWithTwoConstructorDependencies>();

            dep.Validate();
        }

        [Fact]
        public void CanResolveTypeMapping()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            ILogger logger = container.Resolve<ILogger>();

            Assert.NotNull(logger);
            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [Fact]
        public void CanRegisterTypeMappingsWithNames()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special");

            ILogger defaultLogger = container.Resolve<ILogger>();
            ILogger specialLogger = container.Resolve<ILogger>("special");

            Assert.NotNull(defaultLogger);
            Assert.NotNull(specialLogger);

            AssertExtensions.IsInstanceOfType(defaultLogger, typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(specialLogger, typeof(SpecialLogger));
        }

        [Fact]
        public void ShouldDoPropertyInjection()
        {
            IUnityContainer container = new UnityContainer();

            ObjectWithTwoProperties obj = container.Resolve<ObjectWithTwoProperties>();

            obj.Validate();
        }

        [Fact]
        public void ShouldSkipIndexers()
        {
            IUnityContainer container = new UnityContainer();

            ObjectWithIndexer obj = container.Resolve<ObjectWithIndexer>();

            obj.Validate();
        }

        [Fact]
        public void ShouldSkipStaticProperties()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<object>(this);

            var obj = container.Resolve<ObjectWithStaticAndInstanceProperties>();

            obj.Validate();
        }

        [Fact]
        public void ShouldDoAllInjections()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            ObjectWithLotsOfDependencies obj = container.Resolve<ObjectWithLotsOfDependencies>();

            Assert.NotNull(obj);
            obj.Validate();
        }

        [Fact]
        public void CanGetObjectsUsingNongenericMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger));

            object logger = container.Resolve(typeof(ILogger));

            Assert.NotNull(logger);
            AssertExtensions.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [Fact]
        public void CanGetNamedObjectsUsingNongenericMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger))
                .RegisterType(typeof(ILogger), typeof(SpecialLogger), "special");

            ILogger defaultLogger = container.Resolve(typeof(ILogger)) as ILogger;
            ILogger specialLogger = container.Resolve(typeof(ILogger), "special") as ILogger;

            Assert.NotNull(defaultLogger);
            Assert.NotNull(specialLogger);

            AssertExtensions.IsInstanceOfType(defaultLogger, typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(specialLogger, typeof(SpecialLogger));
        }

        [Fact]
        public void AllInjectionsWorkFromNongenericMethods()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ILogger), typeof(MockLogger));

            ObjectWithLotsOfDependencies obj = (ObjectWithLotsOfDependencies)container.Resolve(typeof(ObjectWithLotsOfDependencies));
            obj.Validate();
        }

        [Fact]
        public void ContainerSupportsSingletons()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager());

            ILogger logger1 = container.Resolve<ILogger>();
            ILogger logger2 = container.Resolve<ILogger>();

            AssertExtensions.IsInstanceOfType(logger1, typeof(MockLogger));
            Assert.Same(logger1, logger2);
        }

        [Fact]
        public void CanCreatedNamedSingletons()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            ILogger logger1 = container.Resolve<ILogger>();
            ILogger logger2 = container.Resolve<ILogger>();
            ILogger logger3 = container.Resolve<ILogger>("special");
            ILogger logger4 = container.Resolve<ILogger>("special");

            Assert.NotSame(logger1, logger2);
            Assert.Same(logger3, logger4);
        }

        [Fact]
        public void CanRegisterSingletonsWithNongenericMethods()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>(new ContainerControlledLifetimeManager())
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            ILogger logger1 = container.Resolve<ILogger>();
            ILogger logger2 = container.Resolve<ILogger>();
            ILogger logger3 = container.Resolve<ILogger>("special");
            ILogger logger4 = container.Resolve<ILogger>("special");

            Assert.Same(logger1, logger2);
            Assert.Same(logger3, logger4);
            Assert.NotSame(logger1, logger3);
        }

        [Fact]
        public void DisposingContainerDisposesSingletons()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<DisposableObject>(new ContainerControlledLifetimeManager());

            DisposableObject dobj = container.Resolve<DisposableObject>();

            Assert.False(dobj.WasDisposed);
            container.Dispose();

            Assert.True(dobj.WasDisposed);
        }

        [Fact]
        public void SingletonsRegisteredAsDefaultGetInjected()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ObjectWithOneDependency>(new ContainerControlledLifetimeManager());

            ObjectWithOneDependency dep = container.Resolve<ObjectWithOneDependency>();
            ObjectWithTwoConstructorDependencies dep2 =
                container.Resolve<ObjectWithTwoConstructorDependencies>();

            Assert.Same(dep, dep2.OneDep);
        }

        [Fact]
        public void CanDoInjectionOnExistingObjects()
        {
            IUnityContainer container = new UnityContainer();

            ObjectWithTwoProperties o = new ObjectWithTwoProperties();
            Assert.Null(o.Obj1);
            Assert.Null(o.Obj2);

            container.BuildUp(o);

            o.Validate();
        }

        [Fact]
        public void CanBuildUpExistingObjectWithNonGenericObject()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            ObjectUsingLogger o = new ObjectUsingLogger();
            object result = container.BuildUp(o);

            AssertExtensions.IsInstanceOfType(result, typeof(ObjectUsingLogger));
            Assert.Same(o, result);
            Assert.NotNull(o.Logger);
            AssertExtensions.IsInstanceOfType(o.Logger, typeof(MockLogger));
        }

        [Fact]
        public void CanBuildupObjectWithExplicitInterface()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            ObjectWithExplicitInterface o = new ObjectWithExplicitInterface();
            container.BuildUp<ISomeCommonProperties>(o);

            o.ValidateInterface();
        }

        [Fact]
        public void CanBuildupObjectWithExplicitInterfaceUsingNongenericMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>();

            ObjectWithExplicitInterface o = new ObjectWithExplicitInterface();
            container.BuildUp(typeof(ISomeCommonProperties), o);

            o.ValidateInterface();
        }

        [Fact]
        public void CanUseInstanceAsSingleton()
        {
            MockLogger logger = new MockLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(ILogger), "logger", logger, new ContainerControlledLifetimeManager());

            ILogger o = container.Resolve<ILogger>("logger");
            Assert.Same(logger, o);
        }

        [Fact]
        public void CanUseInstanceAsSingletonViaGenericMethod()
        {
            MockLogger logger = new MockLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ILogger>("logger", logger);

            ILogger o = container.Resolve<ILogger>("logger");
            Assert.Same(logger, o);
        }

        [Fact]
        public void DisposingContainerDisposesOwnedInstances()
        {
            DisposableObject o = new DisposableObject();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(object), o);

            container.Dispose();
            Assert.True(o.WasDisposed);
        }

        [Fact]
        public void DisposingContainerDoesNotDisposeUnownedInstances()
        {
            DisposableObject o = new DisposableObject();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(object), o, new ExternallyControlledLifetimeManager());

            container.Dispose();
            Assert.False(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [Fact]
        public void ContainerDefaultsToInstanceOwnership()
        {
            DisposableObject o = new DisposableObject();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(object), o);
            container.Dispose();
            Assert.True(o.WasDisposed);
        }

        [Fact]
        public void ContainerDefaultsToInstanceOwnershipViaGenericMethod()
        {
            DisposableObject o = new DisposableObject();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(DisposableObject), o);
            container.Dispose();
            Assert.True(o.WasDisposed);
        }

        [Fact]
        public void InstanceRegistrationWithoutNameRegistersDefault()
        {
            MockLogger l = new MockLogger();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(ILogger), l);

            ILogger o = container.Resolve<ILogger>();
            Assert.Same(l, o);
        }

        [Fact]
        public void InstanceRegistrationWithoutNameRegistersDefaultViaGenericMethod()
        {
            MockLogger l = new MockLogger();
            IUnityContainer container = new UnityContainer()
                .RegisterInstance<ILogger>(l);

            ILogger o = container.Resolve<ILogger>();
            Assert.Same(l, o);
        }

        [Fact]
        public void CanRegisterDefaultInstanceWithoutLifetime()
        {
            DisposableObject o = new DisposableObject();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(object), o, new ExternallyControlledLifetimeManager());

            object result = container.Resolve<object>();
            Assert.NotNull(result);
            Assert.Same(o, result);

            container.Dispose();
            Assert.False(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [Fact]
        public void CanRegisterDefaultInstanceWithoutLifetimeViaGenericMethod()
        {
            DisposableObject o = new DisposableObject();

            IUnityContainer container = new UnityContainer()
                .RegisterInstance<object>(o, new ExternallyControlledLifetimeManager());

            object result = container.Resolve<object>();
            Assert.NotNull(result);
            Assert.Same(o, result);

            container.Dispose();
            Assert.False(o.WasDisposed);
            GC.KeepAlive(o);
        }

        [Fact]
        public void CanSpecifyInjectionConstructorWithDefaultDependencies()
        {
            string sampleString = "Hi there";
            IUnityContainer container = new UnityContainer()
                .RegisterInstance(sampleString);

            ObjectWithInjectionConstructor o = container.Resolve<ObjectWithInjectionConstructor>();

            Assert.NotNull(o.ConstructorDependency);
            Assert.Same(sampleString, o.ConstructorDependency);
        }

        [Fact]
        public void CanGetInstancesOfAllRegisteredTypes()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("mock")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>("another");

            List<ILogger> loggers = new List<ILogger>(
                container.ResolveAll<ILogger>());

            Assert.Equal(3, loggers.Count);
            AssertExtensions.IsInstanceOfType(loggers[0], typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            AssertExtensions.IsInstanceOfType(loggers[2], typeof(MockLogger));
        }

        [Fact]
        public void GetAllDoesNotReturnTheDefault()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>();

            List<ILogger> loggers = new List<ILogger>(
                container.ResolveAll<ILogger>());
            Assert.Equal(1, loggers.Count);
            AssertExtensions.IsInstanceOfType(loggers[0], typeof(SpecialLogger));
        }

        [Fact]
        public void CanGetAllWithNonGenericMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("mock")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterType<ILogger, MockLogger>("another");

            List<object> loggers = new List<object>(
                container.ResolveAll(typeof(ILogger)));

            Assert.Equal(3, loggers.Count);
            AssertExtensions.IsInstanceOfType(loggers[0], typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            AssertExtensions.IsInstanceOfType(loggers[2], typeof(MockLogger));
        }

        [Fact]
        public void GetAllReturnsRegisteredInstances()
        {
            MockLogger l = new MockLogger();

            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>("normal")
                .RegisterType<ILogger, SpecialLogger>("special")
                .RegisterInstance<ILogger>("instance", l);

            List<ILogger> loggers = new List<ILogger>(
                container.ResolveAll<ILogger>());

            Assert.Equal(3, loggers.Count);
            AssertExtensions.IsInstanceOfType(loggers[0], typeof(MockLogger));
            AssertExtensions.IsInstanceOfType(loggers[1], typeof(SpecialLogger));
            Assert.Same(l, loggers[2]);
        }

        [Fact]
        public void CanRegisterLifetimeAsSingleton()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType<ILogger, MockLogger>()
                .RegisterType<ILogger, SpecialLogger>("special", new ContainerControlledLifetimeManager());

            ILogger logger1 = container.Resolve<ILogger>();
            ILogger logger2 = container.Resolve<ILogger>();
            ILogger logger3 = container.Resolve<ILogger>("special");
            ILogger logger4 = container.Resolve<ILogger>("special");

            Assert.NotSame(logger1, logger2);
            Assert.Same(logger3, logger4);
        }

        [Fact]
        public void ShouldThrowIfAttemptsToResolveUnregisteredInterface()
        {
            IUnityContainer container = new UnityContainer();

            AssertExtensions.AssertException<ResolutionFailedException>(() =>
                {
                    container.Resolve<ILogger>();
                });
        }

        [Fact]
        public void CanBuildSameTypeTwice()
        {
            IUnityContainer container = new UnityContainer();

            container.Resolve<ObjectWithTwoConstructorDependencies>();
            container.Resolve<ObjectWithTwoConstructorDependencies>();
        }

        [Fact]
        public void CanRegisterMultipleStringInstances()
        {
            IUnityContainer container = new UnityContainer();
            string first = "first";
            string second = "second";

            container.RegisterInstance<string>(first)
                .RegisterInstance<string>(second);

            string result = container.Resolve<string>();

            Assert.Equal(second, result);
        }

        [Fact]
        public void GetReasonableExceptionWhenRegisteringNullInstance()
        {
            IUnityContainer container = new UnityContainer();
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    container.RegisterInstance<SomeType>(null);
                });
        }

        [Fact]
        public void RegisteringTheSameLifetimeManagerTwiceThrows()
        {
            LifetimeManager singleton = new ContainerControlledLifetimeManager();

            AssertExtensions.AssertException<InvalidOperationException>(() =>
                {
                    new UnityContainer()
                        .RegisterType<ILogger, MockLogger>(singleton)
                        .RegisterType<ILogger, SpecialLogger>("special", singleton);
                });
        }

        [Fact]
        public void CanRegisterGenericTypesAndResolveThem()
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            myDict.Add("One", "two");
            myDict.Add("Two", "three");

            IUnityContainer container = new UnityContainer()
                .RegisterInstance(myDict)
                .RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));

            IDictionary<string, string> result = container.Resolve<IDictionary<string, string>>();
            Assert.Same(myDict, result);
        }

        [Fact]
        public void CanSpecializeGenericsViaTypeMappings()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(IRepository<>), typeof(MockRespository<>))
                .RegisterType<IRepository<SomeType>, SomeTypRepository>();

            IRepository<string> generalResult = container.Resolve<IRepository<string>>();
            IRepository<SomeType> specializedResult = container.Resolve<IRepository<SomeType>>();

            AssertExtensions.IsInstanceOfType(generalResult, typeof(MockRespository<string>));
            AssertExtensions.IsInstanceOfType(specializedResult, typeof(SomeTypRepository));
        }

        [Fact]
        public void ContainerResolvesItself()
        {
            IUnityContainer container = new UnityContainer();

            Assert.Same(container, container.Resolve<IUnityContainer>());
        }

        [Fact]
        public void ContainerResolvesItselfEvenAfterGarbageCollect()
        {
            IUnityContainer container = new UnityContainer();
            container.AddNewExtension<GarbageCollectingExtension>();

            Assert.NotNull(container.Resolve<IUnityContainer>());
        }

        public class GarbageCollectingExtension : UnityContainerExtension
        {
            protected override void Initialize()
            {
                this.Context.Strategies.AddNew<GarbageCollectingStrategy>(UnityBuildStage.Setup);
            }

            public class GarbageCollectingStrategy : BuilderStrategy
            {
                public override void PreBuildUp(IBuilderContext context)
                {
                    GC.Collect();
                }
            }
        }

        [Fact]
        public void ChildContainerResolvesChildNotParent()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            Assert.Same(child, child.Resolve<IUnityContainer>());
        }

        [Fact]
        public void ParentContainerResolvesParentNotChild()
        {
            IUnityContainer parent = new UnityContainer();
            IUnityContainer child = parent.CreateChildContainer();

            Assert.Same(parent, parent.Resolve<IUnityContainer>());
        }

        [Fact]
        public void ResolvingOpenGenericGivesInnerInvalidOperationException()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor(10));

            AssertExtensions.AssertException<ResolutionFailedException>(
                () => { container.Resolve(typeof(List<>)); },
                (e) => { AssertExtensions.IsInstanceOfType(e.InnerException, typeof(ArgumentException)); });
        }

        [Fact]
        public void ResovingObjectWithPrivateSetterGivesUsefulException()
        {
            IUnityContainer container = new UnityContainer();

            AssertExtensions.AssertException<ResolutionFailedException>(
                () => { container.Resolve<ObjectWithPrivateSetter>(); },
                (e) => { AssertExtensions.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException)); });
        }

        [Fact]
        public void ResolvingUnconfiguredPrimitiveDependencyGivesReasonableException()
        {
            ResolvingUnconfiguredPrimitiveGivesResonableException<string>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<bool>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<char>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<float>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<double>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<byte>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<short>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<int>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<long>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<IntPtr>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<UIntPtr>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<ushort>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<uint>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<ulong>();
            ResolvingUnconfiguredPrimitiveGivesResonableException<sbyte>();
        }

        private void ResolvingUnconfiguredPrimitiveGivesResonableException<T>()
        {
            IUnityContainer container = new UnityContainer();
            try
            {
                container.Resolve<TypeWithPrimitiveDependency<T>>();
            }
            catch (ResolutionFailedException e)
            {
                AssertExtensions.IsInstanceOfType(e.InnerException, typeof(InvalidOperationException));
                return;
            }
            Assert.True(false, string.Format("Expected exception did not occur"));
        }

        internal class SomeType
        {
        }

        public interface IRepository<TEntity>
        {
        }

        public class MockRespository<TEntity> : IRepository<TEntity>
        {
        }

        public class SomeTypRepository : IRepository<SomeType>
        {
        }

        public class ObjectWithPrivateSetter
        {
            [Dependency]
            public object Obj1 { get; private set; }
        }

        public class TypeWithPrimitiveDependency<T>
        {
            public TypeWithPrimitiveDependency(T dependency)
            {
            }
        }
    }
}
