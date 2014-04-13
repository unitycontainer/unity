// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class DependencyArrayAttributeFixture
    {
        [TestMethod]
        public void CanResolveArrayForConstructorParameter()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod]
        public void CanResolveArrayForProperty()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayProperty resolved = container.Resolve<TypeWithArrayProperty>();

            Assert.IsNotNull(resolved.Loggers);
            Assert.AreEqual(2, resolved.Loggers.Length);
            Assert.AreSame(o1, resolved.Loggers[0]);
            Assert.AreSame(o2, resolved.Loggers[1]);
        }

        [TestMethod]
        public void CanResolveArrayForConstructorParameterOnClosedGenericType()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            GenericTypeWithArrayConstructorParameter<ILogger> resolved
                = container.Resolve<GenericTypeWithArrayConstructorParameter<ILogger>>();

            Assert.IsNotNull(resolved.Values);
            Assert.AreEqual(2, resolved.Values.Length);
            Assert.AreSame(o1, resolved.Values[0]);
            Assert.AreSame(o2, resolved.Values[1]);
        }

        [TestMethod]
        public void BindingDependencyArrayToArrayParameterWithRankOverOneThrows()
        {
            IUnityContainer container = new UnityContainer();

            try
            {
                container.Resolve<TypeWithArrayConstructorParameterOfRankTwo>();
                Assert.Fail("Call to Resolve<>() should have failed");
            }
            catch (ResolutionFailedException)
            {
            }
        }

        public class TypeWithArrayConstructorParameter
        {
            public readonly ILogger[] Loggers;

            public TypeWithArrayConstructorParameter(ILogger[] loggers)
            {
                this.Loggers = loggers;
            }
        }

        public class GenericTypeWithArrayConstructorParameter<T>
        {
            public readonly T[] Values;

            public GenericTypeWithArrayConstructorParameter(T[] values)
            {
                this.Values = values;
            }
        }

        public class TypeWithArrayProperty
        {
            private ILogger[] loggers;

            [Dependency]
            public ILogger[] Loggers
            {
                get { return loggers; }
                set { this.loggers = value; }
            }
        }

        public class TypeWithArrayConstructorParameterOfRankTwo
        {
            public TypeWithArrayConstructorParameterOfRankTwo(ILogger[,] loggers)
            {
            }
        }
    }
}
