// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ObjectBuilder2;
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
     
    public class DependencyArrayAttributeFixture
    {
        [Fact]
        public void CanResolveArrayForConstructorParameter()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayConstructorParameter resolved = container.Resolve<TypeWithArrayConstructorParameter>();

            Assert.NotNull(resolved.Loggers);
            Assert.Equal(2, resolved.Loggers.Length);
            Assert.Same(o1, resolved.Loggers[0]);
            Assert.Same(o2, resolved.Loggers[1]);
        }

        [Fact]
        public void CanResolveArrayForProperty()
        {
            ILogger o1 = new MockLogger();
            ILogger o2 = new SpecialLogger();

            IUnityContainer container
                = new UnityContainer()
                .RegisterInstance<ILogger>("o1", o1)
                .RegisterInstance<ILogger>("o2", o2);

            TypeWithArrayProperty resolved = container.Resolve<TypeWithArrayProperty>();

            Assert.NotNull(resolved.Loggers);
            Assert.Equal(2, resolved.Loggers.Length);
            Assert.Same(o1, resolved.Loggers[0]);
            Assert.Same(o2, resolved.Loggers[1]);
        }

        [Fact]
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

            Assert.NotNull(resolved.Values);
            Assert.Equal(2, resolved.Values.Length);
            Assert.Same(o1, resolved.Values[0]);
            Assert.Same(o2, resolved.Values[1]);
        }

        [Fact]
        public void BindingDependencyArrayToArrayParameterWithRankOverOneThrows()
        {
            IUnityContainer container = new UnityContainer();

            try
            {
                container.Resolve<TypeWithArrayConstructorParameterOfRankTwo>();
                Assert.True(false, string.Format("Call to Resolve<>() should have failed"));
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
