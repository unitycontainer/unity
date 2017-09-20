// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.StaticFactory;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
#pragma warning disable CS0618 // Type or member is obsolete

    /// <summary>
    /// Summary description for StaticFactoryFixture
    /// </summary>
    /// 
    public interface ITestInterface
    {
    }

    public class TestClass : ITestInterface
    {
        public string Str = "check";
    }

    [TestClass]
    public class StaticFactoryFixture
    {
        [TestMethod]
        public void AddNewExtensionToUnityContainer()
        {
            //unitycontainer :Creates a default UnityContainer. Returns a reference to the new container 
            //as an instance of the IUnityContainer interface.
            //AddNewExtension : Creates a new extension object of type TExtension and adds it to the container. The 
            //extension type must have a zero-argument public constructor. Returns a reference to the container
            IUnityContainer container = new UnityContainer().AddNewExtension<StaticFactoryExtension>();
            // container.Register<Container, UnityContainer>;
            Assert.IsNotNull(container);

            container = container.RemoveAllExtensions();
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void ConfigureAndRegisterInstance()
        {
            IUnityContainer container = new UnityContainer().AddNewExtension<StaticFactoryExtension>();
            IStaticFactoryConfiguration config = container.Configure<IStaticFactoryConfiguration>();
            object obj1 = container.RegisterInstance<IStaticFactoryConfiguration>(config);
            Assert.IsNotNull(config);
            Assert.IsNotNull(obj1);
        }

        [TestMethod]
        public void UseRegisterFactoryTwoParameters()
        {
            IUnityContainer container = new UnityContainer().AddNewExtension<StaticFactoryExtension>();

            //Returns the configuration of the specified extension interface as an object of type TConfigurator,
            //or null if the specified extension interface is not found. Extensions can expose configuration 
            //interfaces as well as adding strategies and policies to the container. This method walks the 
            //list of extensions and returns the first one that implements the specified type.
            //Static Factory Test Cases.
            //(when you deal with object you can’t control; you provide a delegate that calls the factory) 

            IStaticFactoryConfiguration config = container.Configure<IStaticFactoryConfiguration>();
            object obj1 = config.RegisterFactory<TestClass>("Check", delegate
                                                                   { return new TestClass(); });
            Assert.IsNotNull(config);
            Assert.IsNotNull(obj1);
        }

        [TestMethod]
        public void UseRegisterFactoryOneParameters()
        {
            IUnityContainer container = new UnityContainer().AddNewExtension<StaticFactoryExtension>();
            //Returns the configuration of the specified extension interface as an object of type TConfigurator,
            //or null if the specified extension interface is not found. Extensions can expose configuration 
            //interfaces as well as adding strategies and policies to the container. This method walks the 
            //list of extensions and returns the first one that implements the specified type.
            IStaticFactoryConfiguration config = container.Configure<IStaticFactoryConfiguration>();
            object obj1 = config.RegisterFactory<TestClass>(delegate { return new TestClass(); });

            Assert.IsNotNull(config);
            Assert.IsNotNull(obj1);
        }

        [TestMethod]
        public void FactoryDelegateGetsCalledRegistorFactoryOneParam()
        {
            bool check = false;

            IUnityContainer container = new UnityContainer()
                .AddNewExtension<StaticFactoryExtension>()
                .Configure<IStaticFactoryConfiguration>()
                    .RegisterFactory<TestClass>(delegate(IUnityContainer uc)
                        {
                            check = true;
                            return new TestClass();
                        })
                    .Container.RegisterType<ITestInterface, TestClass>();

            ITestInterface testInt = container.Resolve<ITestInterface>();
            Assert.IsInstanceOfType(testInt, typeof(TestClass));
            Assert.IsTrue(check);
        }

        [TestMethod]
        public void FactoryDelegateGetsCalledRegistorFactoryTwoParam()
        {
            bool blncheck = false;
            string str = null;
            //StaticFactoryExtension extension = new StaticFactoryExtension();
            IUnityContainer container = new UnityContainer()
                                            .AddNewExtension<StaticFactoryExtension>()
                                            .Configure<IStaticFactoryConfiguration>()
                                            .RegisterFactory<TestClass>(str, delegate(IUnityContainer uc)
                                                                                {
                                                                                    blncheck = true;
                                                                                    return new TestClass();
                                                                                })
                                            .Container.RegisterType<ITestInterface, TestClass>();
            ITestInterface testInt = container.Resolve<ITestInterface>();
            Assert.IsInstanceOfType(testInt, typeof(TestClass));
            Assert.IsTrue(blncheck);
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
