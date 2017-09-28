// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests.Extension
{
    /// <summary>
    /// Summary description for MyCustomExtensionFixture
    /// </summary>
    [TestClass]
    public class MyCustomExtensionFixture
    {
        /// <summary>
        /// Create instance of MyCustomExtension and add it to the UnityContainer
        /// </summary>
        [TestMethod]
        public void AddExistingMyCustonExtensionToContainer()
        {
            MyCustomExtension extension = new MyCustomExtension();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            Assert.IsNotNull(uc);
            Assert.IsTrue(extension.CheckPolicyValue);
        }

        /// <summary>
        /// Add the MyCustomExtension to the UnityContainer
        /// </summary>
        [TestMethod]
        public void AddMyCustonExtensionToContainer()
        {
            IUnityContainer uc = new UnityContainer();
            uc.AddNewExtension<MyCustomExtension>();

            Assert.IsNotNull(uc);
        }

        /// <summary>
        /// Check whether extension is added to the container created.
        /// </summary>
        [TestMethod]
        public void CheckExtensionAddedToContainer()
        {
            MyCustomExtension extension = new MyCustomExtension();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            Assert.AreSame(uc, extension.Container);
        }

        /// <summary>
        /// Check whether extension is added to the container created.
        /// </summary>
        [TestMethod]
        public void CheckExtensionAdded()
        {
            MyCustomExtension extension = new MyCustomExtension();
            IUnityContainer uc = new UnityContainer();
            uc.AddExtension(extension);

            Assert.IsTrue(extension.CheckExtensionValue);
        }

        /// <summary>
        /// Add extension to the conatiner. Check if object is returned. 
        /// </summary>
        [TestMethod]
        public void AddExtensionGetObject()
        {
            MyCustomExtension extension = new MyCustomExtension();

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Get access to the configuration interface given by the extension.
        /// </summary>
        [TestMethod]
        public void ConfigureToContainer()
        {
            MyCustomExtension extension = new MyCustomExtension();

            IUnityContainer container = new UnityContainer()
                .AddExtension(extension);

            IMyCustomConfigurator extend = container.Configure<IMyCustomConfigurator>();

            Assert.AreSame(extension.Container, extend.Container);
        }

        /// <summary>
        /// Add new extension, access the configurator exposed by extension, add a new policy.
        /// </summary>
        [TestMethod]
        public void AddExtensionAddPolicyAddConfigurator()
        {
            IUnityContainer container = new UnityContainer()
                 .AddNewExtension<MyCustomExtension>()
                 .Configure<IMyCustomConfigurator>()
                     .AddPolicy().Container;

            Assert.IsNotNull(container);
        }

        /// <summary>
        /// Add existing instance of extension, access the configurator exposed by extension, add a new policy.
        /// </summary>
        [TestMethod]
        public void AddExistExtensionAddPolicyAddConfigurator()
        {
            MyCustomExtension extension = new MyCustomExtension();
            IUnityContainer container = new UnityContainer()
                 .AddExtension(extension)
                 .Configure<IMyCustomConfigurator>()
                     .AddPolicy().Container;

            Assert.IsNotNull(container);
            Assert.IsTrue(extension.CheckPolicyValue);
        }

        /// <summary>
        /// Remove all extensions from the container
        /// </summary>
        [TestMethod]
        public void RemoveAllExtensions()
        {
            IUnityContainer container = new UnityContainer()
                .RemoveAllExtensions();

            // Default behavior should always work
            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Remove all extensions. then add the default extension to the container.
        /// </summary>
        [TestMethod]
        public void AddDefaultExtensions()
        {
            IUnityContainer container = new UnityContainer()
                .RemoveAllExtensions();

            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Remove all extensions. Add default extension and the new extension.
        /// </summary>
        [TestMethod]
        public void AddDefaultAndCustomExtensions()
        {
            IUnityContainer container = new UnityContainer()
                .RemoveAllExtensions()
                .AddExtension(new MyCustomExtension());

            object result = container.Resolve<object>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(container);
        }

        /// <summary>
        /// Add existing instance of extension. SetLifetime of the extension with the container.
        /// </summary>       
        [TestMethod]
        public void AddExtensionSetLifetime()
        {
            MyCustomExtension extension = new MyCustomExtension();
            IUnityContainer container = new UnityContainer()
                 .AddExtension(extension);

            container.RegisterInstance<MyCustomExtension>(extension);
            object result = container.Resolve<object>();
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(container);
        }
    }
}