// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for DependencyOverrideFixture
    /// </summary>
    [TestClass]
    public class DependencyOverrideFixture : ConfigurationFixtureBase
    {
        private const string ConfigFileName = @"ConfigFiles\DependencyOverride.config";

        public DependencyOverrideFixture()
            : base(ConfigFileName)
        {
        }

        [TestMethod]
        public void DependencyOverridesAllMatchTest()
        {
            IToWhichDependent defaultValue = new Type1ToWhichDependent(111);
            IToWhichDependent overrideValue = new Type2ToWhichDependent(222);

            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111));
            Type1DependingOnOtherType result = (Type1DependingOnOtherType)container.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue));
            
            Assert.IsInstanceOfType(result.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(result.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void DependencyOverridesTypeMismatchTest()
        {
            IToWhichDependent defaultValue = new Type1ToWhichDependent(111);
            IToWhichDependent overrideValue = new Type2ToWhichDependent(222);
            
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111));
            Type1DependingOnOtherType result = (Type1DependingOnOtherType)container.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(int), overrideValue));
        }

       
        [TestMethod]
        public void DependencyOverrideWithConstructorDependency()
        {
            // Using the [Dependency] attribute in to the constructor parameter.
            IToWhichDependent overrideValue = new Type2ToWhichDependent(222);
            
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<IDependingOnOtherType, Type3DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(999));
            Type3DependingOnOtherType result = (Type3DependingOnOtherType)container.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue));
            
            Assert.IsInstanceOfType(result.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(result.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
        }

        [TestMethod]
        public void WhenDenpendencyOverridesOccursOnPropertyWithPrivateSetter()
        {
            ClassWithPrivateSetterDependency defaultValue = new ClassWithPrivateSetterDependency();

            IUnityContainer container = new UnityContainer();
            container.RegisterType<ClassWithPrivateSetterDependency>();

            try
            {
                var result = container.Resolve<ClassWithPrivateSetterDependency>(new DependencyOverride(typeof(string), "value"));
                Assert.Fail("Should have failed");
            }
            catch (ResolutionFailedException resolutionFailedException)
            {
                Assert.AreEqual(typeof(InvalidOperationException), resolutionFailedException.InnerException.GetType());
            }
        }

        [TestMethod]
        public void NestedContainerOverrideInParentDoesNotReturnOverridenInChildAcrossCalls()
        {
            IToWhichDependent overrideValue = new Type2ToWhichDependent(222);

            IUnityContainer container = new UnityContainer();
            IUnityContainer childContainer = container.CreateChildContainer();

            // Registering the parent default types.
            container.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111));

            // Overriding the default values for the parent container.
            Type1DependingOnOtherType parentResult = (Type1DependingOnOtherType)container.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue));
            // Resolving child container to the default type.
            Type1DependingOnOtherType childResult = (Type1DependingOnOtherType)childContainer.Resolve<IDependingOnOtherType>();

            // The parent overriden values should be reflected.
            Assert.IsInstanceOfType(parentResult.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(parentResult.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(parentResult.IToWhichDependent.X, 222);

            // The parent default registered type should be reflected, since there are no type registered exclusively for the child container.
            Assert.IsInstanceOfType(childResult.IToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.IsInstanceOfType(childResult.OneMoreIToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.AreEqual<int>(childResult.IToWhichDependent.X, 111);
        }

        [TestMethod]
        public void NestedContainerOverrideInChildDoesNotOverrideInParentAcrossCalls()
        {
            IToWhichDependent overrideValue = new Type2ToWhichDependent(222);

            IUnityContainer container = new UnityContainer();
            IUnityContainer childContainer = container.CreateChildContainer();

            // Registering the parent default types.
            container.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111));

            // Overriding the default values for the child container.
            Type1DependingOnOtherType childResult = (Type1DependingOnOtherType)childContainer.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue));
            // Resolving parent container to the default type.
            Type1DependingOnOtherType parentResult = (Type1DependingOnOtherType)container.Resolve<IDependingOnOtherType>();

            // The default types registered to the parent should be reflected.
            Assert.IsInstanceOfType(parentResult.IToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.IsInstanceOfType(parentResult.OneMoreIToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.AreEqual<int>(parentResult.IToWhichDependent.X, 111);

            // The child overriden values should be reflected.
            Assert.IsInstanceOfType(childResult.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(childResult.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(childResult.IToWhichDependent.X, 222);
        }

        [TestMethod]
        public void DependencyOverrideWithNestedContainer()
        {
            IToWhichDependent defaultValue = new Type1ToWhichDependent(111);
            IToWhichDependent overrideValue3 = new Type2ToWhichDependent(333);
            IToWhichDependent overrideValue4 = new Type2ToWhichDependent(444);

            IUnityContainer container = new UnityContainer();
            IUnityContainer childContainer = container.CreateChildContainer();

            // Registering the parent default types.
            container.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111));
            // Registering the child default types.
            childContainer.RegisterType<IDependingOnOtherType, Type1DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(222));

            // Overriding the default values for the parent container.
            Type1DependingOnOtherType parentResult = (Type1DependingOnOtherType)container.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue3));
            // Overriding the default values for the child container.
            Type1DependingOnOtherType childResult = (Type1DependingOnOtherType)childContainer.Resolve<IDependingOnOtherType>(new DependencyOverride(typeof(IToWhichDependent), overrideValue4));

            // The parent overriden values should be reflected.
            Assert.IsInstanceOfType(parentResult.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(parentResult.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(parentResult.IToWhichDependent.X, 333);

            // The child overriden values should be reflected.
            Assert.IsInstanceOfType(childResult.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(childResult.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(childResult.IToWhichDependent.X, 444);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void DependencyOverrideValueInConfigFile()
        {
            IToWhichDependent overrideDependency = new Type2ToWhichDependent(9999);
            DependencyOverride<IToWhichDependent> overrideParam = new DependencyOverride<IToWhichDependent>(overrideDependency);

            IUnityContainer container = GetContainer("DependencyOverrideContainer");

            var result = container.Resolve<Type3DependingOnOtherType>("TestDependencyOverrideDefaultInConfiguration", overrideParam);
            
            Assert.IsInstanceOfType(result.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(result.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(9999, result.OneMoreIToWhichDependent.X);
            Assert.AreEqual<int>(9999, result.IToWhichDependent.X);
        }

        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigFilesFolder)]
        public void DependencyOverrideValueInConfigFileDoesNotPersistAcrossCalls()
        {
            IToWhichDependent overrideDependency = new Type2ToWhichDependent(9999);
            DependencyOverride<IToWhichDependent> overrideParam = new DependencyOverride<IToWhichDependent>(overrideDependency);

            IUnityContainer container = GetContainer("DependencyOverrideContainer");

            var result = container.Resolve<Type3DependingOnOtherType>("TestDependencyOverrideDefaultInConfiguration", overrideParam);
            var defaultResult = container.Resolve<Type3DependingOnOtherType>("TestDependencyOverrideDefaultInConfiguration");
            
            Assert.IsInstanceOfType(result.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(result.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(9999, result.OneMoreIToWhichDependent.X);
            Assert.AreEqual<int>(9999, result.IToWhichDependent.X);
            Assert.IsInstanceOfType(defaultResult.IToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.IsInstanceOfType(defaultResult.OneMoreIToWhichDependent, typeof(Type1ToWhichDependent));
            Assert.AreEqual<int>(-111, defaultResult.OneMoreIToWhichDependent.X);
            Assert.AreEqual<int>(-111, defaultResult.IToWhichDependent.X);
        }

        [TestMethod]
        public void WhenOverridingMultipleDependencies()
        {
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<Type4DependingOnOtherType>().RegisterType<IToWhichDependent, Type1ToWhichDependent>(new InjectionConstructor(111)).RegisterType<Dependent>(new InjectionProperty("X", 111));
            Dependent overrideDependent1 = new Dependent();
            overrideDependent1.X = 9999;
            IToWhichDependent overrideDependent2 = new Type2ToWhichDependent(8888);
            DependencyOverride<Dependent> overrideParam1 = new DependencyOverride<Dependent>(overrideDependent1);
            DependencyOverride<IToWhichDependent> overrideParam2 = new DependencyOverride<IToWhichDependent>(overrideDependent2);

            var result = container.Resolve<Type4DependingOnOtherType>(overrideParam1, overrideParam2);
            
            Assert.IsInstanceOfType(result.IToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.IsInstanceOfType(result.OneMoreIToWhichDependent, typeof(Type2ToWhichDependent));
            Assert.AreEqual<int>(8888, result.IToWhichDependent.X);
            Assert.AreEqual<int>(8888, result.OneMoreIToWhichDependent.X);
            Assert.AreEqual<int>(9999, result.NewToWhichDependent.X);
        }

        [TestMethod]
        public void DependencyOverrideOccursAcrossObjectGraphReferences()
        {
            MyIndependentFoo overrideFoo = new MyIndependentFoo();
            overrideFoo.X = -9999;

            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<TopLevelDependentFoo>().RegisterType<SecondLevelDependentFoo>().RegisterType<MyIndependentFoo>(new InjectionProperty("X", 111));

            var defaultResult = container.Resolve<TopLevelDependentFoo>();
            var overrideResult = container.Resolve<TopLevelDependentFoo>(new DependencyOverride(typeof(MyIndependentFoo), overrideFoo));
            
            Assert.AreEqual<int>(111, defaultResult.MyIndependentFooTopLevelDependentFoo.X);
            Assert.AreEqual<int>(111, defaultResult.SecondLevelDependentFoo.MyIndependentFooSecondLevelDependentFoo.X);
            Assert.AreEqual<int>(-9999, overrideResult.MyIndependentFooTopLevelDependentFoo.X);
            Assert.AreEqual<int>(-9999, overrideResult.SecondLevelDependentFoo.MyIndependentFooSecondLevelDependentFoo.X);
        }

        [TestMethod]
        public void DependencyOverrideOccursAcrossObjectGraphValues()
        {
            int overrideValue = -9999;
         
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<TopLevelDependentFoo>().RegisterType<SecondLevelDependentFoo>().RegisterType<MyIndependentFoo>(new InjectionProperty("X", 111));

            var defaultResult = container.Resolve<TopLevelDependentFoo>();
            var overrideResult = container.Resolve<TopLevelDependentFoo>(new DependencyOverride(typeof(int), overrideValue));
            
            Assert.AreEqual<int>(111, defaultResult.MyIndependentFooTopLevelDependentFoo.X);
            Assert.AreEqual<int>(111, defaultResult.SecondLevelDependentFoo.MyIndependentFooSecondLevelDependentFoo.X);
            Assert.AreEqual<int>(-9999, overrideResult.MyIndependentFooTopLevelDependentFoo.X);
            Assert.AreEqual<int>(-9999, overrideResult.SecondLevelDependentFoo.MyIndependentFooSecondLevelDependentFoo.X);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CircularDependencyOverrideRaisesException()
        {
            Account account = new Account();
            account.AccountType = "Saving";
            
            IUnityContainer container = new UnityContainer();
            
            container.RegisterType<Person>(new InjectionProperty("Account"), new InjectionProperty("PersonName", "TestDefaultName")).RegisterType<Account>(new InjectionProperty("Person"), new InjectionProperty("AccountType", "Checking"));
            Person result = container.Resolve<Person>(new DependencyOverride(typeof(Account), account));
        }

        [TestMethod]
        public void WhenOverridingDependenciesUsingArbitraryParameters()
        {
            var container = new UnityContainer();

            container
                .RegisterType<ClassWithDependencyOnMyLogger>(new InjectionConstructor(typeof(MyDependentLogger), 10))
                .RegisterType<MyDependentLogger>(new InjectionConstructor(20, "ignored"));

            var instance = container.Resolve<ClassWithDependencyOnMyLogger>(
                new DependencyOverride<int>(new InjectionParameter(50)).OnType<MyDependentLogger>());

            Assert.AreEqual(10, instance.LogLevel);
            Assert.AreEqual(50, instance.MyLogger.LogLevel);
        }
    }

    public class ClassWithDependencyOnMyLogger
    {
        public ClassWithDependencyOnMyLogger(MyDependentLogger myLogger, int logLevel)
        {
            this.MyLogger = myLogger;
            this.LogLevel = logLevel;
        }

        public int LogLevel { get; private set; }

        public MyDependentLogger MyLogger { get; private set; }
    }

    public class MyDependentLogger
    {
        public MyDependentLogger(int logLevel, string message)
        {
            this.LogLevel = logLevel;
        }

        public int LogLevel { get; private set; }
    }

    public class Person
    {
        public Person(string personName)
        {
            PersonName = personName;
        }

        [Dependency]
        public Account Account { get; set; }

        public string PersonName { get; set; }
    }

    public class Account
    {
        public Account()
        {
            AccountType = "Checking";
        }

        [Dependency]
        public Person Person { get; set; }

        public string AccountType { get; set; }
    }

    public class MyIndependentFoo
    {
        [Dependency]
        public int X { get; set; }
    }

    public class SecondLevelDependentFoo
    {
        [Dependency]
        public MyIndependentFoo MyIndependentFooSecondLevelDependentFoo { get; set; }
    }

    public class TopLevelDependentFoo
    {
        [Dependency]
        public MyIndependentFoo MyIndependentFooTopLevelDependentFoo { get; set; }

        [Dependency]
        public SecondLevelDependentFoo SecondLevelDependentFoo { get; set; }
    }

    public interface IToWhichDependent
    {
        int X { get; set; }
    }

    public class Type1ToWhichDependent : IToWhichDependent
    {
        public Type1ToWhichDependent(int x)
        {
            X = x;
        }

        public int X { get; set; }
    }

    public class Type2ToWhichDependent : IToWhichDependent
    {
        public Type2ToWhichDependent(int x)
        {
            X = x;
        }

        public int X { get; set; }
    }

    public interface IDependingOnOtherType
    {
        [Dependency]
        IToWhichDependent IToWhichDependent { get; set; }
    }

    public class Type1DependingOnOtherType : IDependingOnOtherType
    {
        public Type1DependingOnOtherType(IToWhichDependent dependency)
        {
            IToWhichDependent = dependency;
        }

        public IToWhichDependent IToWhichDependent { get; set; }

        [Dependency]
        public IToWhichDependent OneMoreIToWhichDependent { get; set; }
    }

    public class Type2DependingOnOtherType : IDependingOnOtherType
    {
        public Type2DependingOnOtherType(IToWhichDependent dependency)
        {
            IToWhichDependent = dependency;
        }

        public IToWhichDependent IToWhichDependent { get; set; }

        [Dependency]
        public IToWhichDependent OneMoreIToWhichDependent { get; set; }

        [Dependency("Named")]
        public IToWhichDependent NamedToWhichDependent { get; set; }
    }

    public class Type3DependingOnOtherType : IDependingOnOtherType
    {
        public Type3DependingOnOtherType([Dependency]IToWhichDependent dependency)
        {
            IToWhichDependent = dependency;
            NewIToWhichDependent = new Type1ToWhichDependent(999);
        }

        public IToWhichDependent IToWhichDependent { get; set; }
        [Dependency]
        public IToWhichDependent OneMoreIToWhichDependent { get; set; }
        public IToWhichDependent NewIToWhichDependent { get; set; }
    }

    public class Type4DependingOnOtherType : IDependingOnOtherType
    {
        public Type4DependingOnOtherType([Dependency]IToWhichDependent dependency)
        {
            IToWhichDependent = dependency;
        }

        public IToWhichDependent IToWhichDependent { get; set; }

        [Dependency]
        public IToWhichDependent OneMoreIToWhichDependent { get; set; }

        [Dependency]
        public Dependent NewToWhichDependent { get; set; }
    }

    public class Dependent
    {
        public int X { get; set; }
    }

    public class ClassWithPrivateSetterDependency
    {
        private string propertyField = string.Empty;

        [Dependency]
        public string StringProperty
        {
            get { return propertyField; }
            private set { propertyField = value; }
        }
    }
}