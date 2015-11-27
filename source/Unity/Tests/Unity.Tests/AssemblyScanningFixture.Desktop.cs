// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    public partial class AssemblyScanningFixture
    {
        [TestMethod]
        public void GetsTypesFromLoadedAssembliesExcludingSystemAndUnityByDefault()
        {
            // ensure type is loaded
            var ignore = new Unity.Tests.TestNetAssembly.PublicClass1();

            var typesByAssembly = AllClasses.FromLoadedAssemblies().GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly));
        }

        [TestMethod]
        public void GetsTypesFromLoadedAssembliesIncludingSystemIfOverridden()
        {
            // ensure type is loaded
            var ignore = new Unity.Tests.TestNetAssembly.PublicClass1();

            var typesByAssembly = AllClasses.FromLoadedAssemblies(includeSystemAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly));
        }

        [TestMethod]
        public void GetsTypesFromLoadedAssembliesIncludingUnityIfOverridden()
        {
            // ensure type is loaded
            var ignore = new Unity.Tests.TestNetAssembly.PublicClass1();

            var typesByAssembly = AllClasses.FromLoadedAssemblies(includeUnityAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly));
        }

        [TestMethod]
        public void GetsTypesFromLoadedAssembliesIncludingUnityAndSystemIfOverridden()
        {
            // ensure type is loaded
            var ignore = new Unity.Tests.TestNetAssembly.PublicClass1();

            var typesByAssembly = AllClasses.FromLoadedAssemblies(includeSystemAssemblies: true, includeUnityAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly));
        }

        [TestMethod]
        public void GettingTypesFromLoadedAssembliesWithErrorsThrows()
        {
            // ensure type is loaded
            var ignore = new Unity.Tests.TestNetAssembly.PublicClass1();

            try
            {
                var types = AllClasses.FromLoadedAssemblies(skipOnError: false).ToArray();
                Assert.Fail("should have thrown");
            }
            catch (Exception e)
            {
                if (e is AssertFailedException)
                {
                    throw;
                }
            }
        }

        [TestMethod]
        [Priority(-1)]
        public void GetsTypesFromAssembliesLoadedFromBaseFolderExcludingSystemAndUnityByDefault()
        {
            if (!File.Exists("NotReferencedTestNetAssembly.dll"))
            {
                Assert.Inconclusive("The assembly was not found. Run this test without deployment");
            }

            var typesByAssembly = AllClasses.FromAssembliesInBasePath().GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.Any(g => g.Key.GetName().Name == "NotReferencedTestNetAssembly"), "No types for NotReferencedTestNetAssembly");
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly), "No types for TestNetAssembly");
        }

        [TestMethod]
        [Priority(-1)]
        public void GetsTypesFromAssembliesLoadedFromBaseFolderIncludingSystemIfOverridden()
        {
            if (!File.Exists("NotReferencedTestNetAssembly.dll"))
            {
                Assert.Inconclusive("The assembly was not found. Run this test without deployment");
            }

            var typesByAssembly = AllClasses.FromAssembliesInBasePath(includeSystemAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.Any(g => g.Key.GetName().Name == "NotReferencedTestNetAssembly"), "No types for NotReferencedTestNetAssembly");
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly), "No types for TestNetAssembly");
        }

        [TestMethod]
        [Priority(-1)]
        public void GetsTypesFromAssembliesLoadedFromBaseFolderIncludingUnityIfOverridden()
        {
            if (!File.Exists("NotReferencedTestNetAssembly.dll"))
            {
                Assert.Inconclusive("The assembly was not found. Run this test without deployment");
            }

            var typesByAssembly = AllClasses.FromAssembliesInBasePath(includeUnityAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsFalse(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.Any(g => g.Key.GetName().Name == "NotReferencedTestNetAssembly"), "No types for NotReferencedTestNetAssembly");
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly), "No types for TestNetAssembly");
        }

        [TestMethod]
        [Priority(-1)]
        public void GetsTypesFromAssembliesLoadedFromBaseFolderIncludingSystemAndUnityIfOverridden()
        {
            if (!File.Exists("NotReferencedTestNetAssembly.dll"))
            {
                Assert.Inconclusive("The assembly was not found. Run this test without deployment");
            }

            var typesByAssembly = AllClasses.FromAssembliesInBasePath(includeSystemAssemblies: true, includeUnityAssemblies: true).GroupBy(t => t.Assembly).ToDictionary(g => g.Key);

            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Uri).Assembly));
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(IUnityContainer).Assembly));
            Assert.IsTrue(typesByAssembly.Any(g => g.Key.GetName().Name == "NotReferencedTestNetAssembly"), "No types for NotReferencedTestNetAssembly");
            Assert.IsTrue(typesByAssembly.ContainsKey(typeof(Unity.Tests.TestNetAssembly.PublicClass1).Assembly), "No types for TestNetAssembly");
        }

        [TestMethod]
        public void GettingTypesFromAssembliesLoadedFromBaseFolderWithErrorsThrows()
        {
            try
            {
                var types = AllClasses.FromAssembliesInBasePath(skipOnError: false).ToArray();
            }
            catch (Exception e)
            {
                if (e is AssertFailedException)
                {
                    throw;
                }
            }
        }
    }
}