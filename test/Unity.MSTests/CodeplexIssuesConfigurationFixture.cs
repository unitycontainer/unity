// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    [TestClass]
    public class CodeplexIssuesConfigurationFixture
    {
        private const string ConfigFileName = @"ConfigFiles\ChildContainerWierdnessCodeplex.config";

        //http://unity.codeplex.com/Thread/View.aspx?ThreadId=82190
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigurationFixtureBase.ConfigFilesFolder)]
        public void Bug_4709_ParentAndChildResolveWithSameRegistered()
        {
            UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(ConfigFileName);

            IUnityContainer root = new UnityContainer();
            section.Configure(root);

            IUnityContainer child = root.CreateChildContainer();
            section.Configure(child, "ModuleName");

            ISomeInterface obj1 = root.Resolve<ISomeInterface>();
            Assert.AreEqual<int>(1, obj1.Count());

            ISomeInterface obj = child.Resolve<ISomeInterface>();
            Assert.AreEqual<int>(3, obj.Count());
        }

        //variation of http://unity.codeplex.com/Thread/View.aspx?ThreadId=82190
        [TestMethod]
        [DeploymentItem(ConfigFileName, ConfigurationFixtureBase.ConfigFilesFolder)]
        public void Bug_4709_ParentAndChildResolveWithSameRegistered1()
        {
            UnityConfigurationSection section = ConfigurationFixtureBase.GetUnitySection(ConfigFileName);

            IUnityContainer root = new UnityContainer();
            section.Configure(root);

            IUnityContainer child = root.CreateChildContainer();
            section.Configure(child, "ModuleName");

            ISomeInterface obj = child.Resolve<ISomeInterface>();
            Assert.AreEqual<int>(3, obj.Count());

            ISomeInterface obj1 = root.Resolve<ISomeInterface>();
            Assert.AreEqual<int>(1, obj1.Count());
        }

        /// <summary>
        /// http://unity.codeplex.com/WorkItem/View.aspx?WorkItemId=3392
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"ConfigFiles\GenericsWithResolveAllFromConfig.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        public void Bug_4989_GenericsWithResolveAllFromConfig()
        {
            IUnityContainer container = ConfigurationFixtureBase.GetContainer(@"ConfigFiles\GenericsWithResolveAllFromConfig.config", String.Empty);

            var results = container.ResolveAll<IFoo1<string>>();

            Assert.AreEqual(3, results.Count());
        }

        //variation of the above
        [TestMethod]
        [DeploymentItem(@"ConfigFiles\GenericsWithResolveAllFromConfig.config", ConfigurationFixtureBase.ConfigFilesFolder)]
        public void Bug_4989_GenericsWithResolveAllFilters()
        {
            IUnityContainer container =
                ConfigurationFixtureBase.GetContainer(@"ConfigFiles\GenericsWithResolveAllFromConfig.config", String.Empty);

            var results = container.ResolveAll<IFoo1<string>>();
            Assert.AreEqual(3, results.Count());
        }
    }
}
