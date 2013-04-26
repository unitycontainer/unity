//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.Configuration.ConfigurationHelpers;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ResolvingTypesWithoutNamespacesDefined
    /// </summary>
    [TestClass]
    public class When_ResolvingTypesWithoutNamespacesDefined
    {
        private TypeResolverImpl typeResolver;

        [TestInitialize]
        public void Setup()
        {
            var aliases = new Dictionary<string, string>
                {
                    { "dict", typeof(Dictionary<,>).AssemblyQualifiedName },
                    { "ILogger", "Microsoft.Practices.Unity.TestSupport.ILogger, Unity.TestSupport" },
                    { "MockLogger", "Microsoft.Practices.Unity.TestSupport.MockLogger, Unity.TestSupport" }
                };

            var namespaces = new string[0];
            var assemblies = new[] { "System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "Unity.TestSupport", "an invalid assembly name", "invalid, invalid" };

            typeResolver = new TypeResolverImpl(aliases, namespaces, assemblies);
        }

        [TestMethod]
        public void Then_DefaultAliasesResolve()
        {
            var expected = new Dictionary<string, Type>
                {
                    {"sbyte", typeof (sbyte)},
                    {"short", typeof (short)},
                    {"int", typeof (int)},
                    {"integer", typeof (int)},
                    {"long", typeof (long)},
                    {"byte", typeof (byte)},
                    {"ushort", typeof (ushort)},
                    {"uint", typeof (uint)},
                    {"ulong", typeof (ulong)},
                    {"float", typeof (float)},
                    {"single", typeof (float)},
                    {"double", typeof (double)},
                    {"decimal", typeof (decimal)},
                    {"char", typeof (char)},
                    {"bool", typeof (bool)},
                    {"object", typeof (object)},
                    {"string", typeof (string)},
                    {"datetime", typeof (DateTime)},
                    {"DateTime", typeof (DateTime)},
                    {"date", typeof (DateTime)},
                    {"singleton", typeof (ContainerControlledLifetimeManager)},
                    {"ContainerControlledLifetimeManager", typeof (ContainerControlledLifetimeManager)},
                    {"transient", typeof (TransientLifetimeManager)},
                    {"TransientLifetimeManager", typeof (TransientLifetimeManager)},
                    {"perthread", typeof (PerThreadLifetimeManager)},
                    {"PerThreadLifetimeManager", typeof (PerThreadLifetimeManager)},
                    {"external", typeof (ExternallyControlledLifetimeManager)},
                    {"ExternallyControlledLifetimeManager", typeof (ExternallyControlledLifetimeManager)},
                    {"hierarchical", typeof (HierarchicalLifetimeManager)},
                    {"HierarchicalLifetimeManager", typeof (HierarchicalLifetimeManager)},
                    {"resolve", typeof (PerResolveLifetimeManager)},
                    {"perresolve", typeof (PerResolveLifetimeManager)},
                    {"PerResolveLifetimeManager", typeof (PerResolveLifetimeManager)},
                };

            foreach (var kv in expected)
            {
                Assert.AreSame(kv.Value, typeResolver.ResolveType(kv.Key, true));
            }
        }

        [TestMethod]
        public void Then_ILoggerResolves()
        {
            Assert.AreSame(typeResolver.ResolveType("ILogger", true), typeof(ILogger));
        }

        [TestMethod]
        public void Then_ILoggerResolvesThroughSearch()
        {
            Assert.AreSame(typeResolver.ResolveType("Microsoft.Practices.Unity.TestSupport.ILogger", true), typeof(ILogger));
        }

        [TestMethod]
        public void Then_ShorthandForOpenGenericWithOneParameterWorks()
        {
            Assert.AreSame(typeResolver.ResolveType("System.Collections.Generic.List[]", true), typeof(List<>));
        }

        [TestMethod]
        public void Then_ShorthandGenericIsResolved()
        {
            Assert.AreSame(typeResolver.ResolveType("System.Collections.Generic.List[int]", true), typeof(List<int>));
        }

        [TestMethod]
        public void Then_TypeThatCannotBeFoundReturnsNull()
        {
            Assert.IsNull(typeResolver.ResolveType("Namespace.Type, Assembly", false));
        }
    }
}
