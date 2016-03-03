// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ObjectBuilder2;
using Unity.ObjectBuilder;
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
    // Tests for the DependencyValue class and its derivatives
     
    public class InjectionParameterValueFixture
    {
        [Fact]
        public void InjectionParameterReturnsExpectedValue()
        {
            int expected = 12;
            InjectionParameter parameter = new InjectionParameter(expected);
            AssertExpectedValue(parameter, typeof(int), expected);
        }

        [Fact]
        public void InjectionParameterCanTakeExplicitType()
        {
            double expected = Math.E;
            InjectionParameter parameter = new InjectionParameter<double>(expected);
            AssertExpectedValue(parameter, typeof(double), expected);
        }

        [Fact]
        public void InjectionParameterCanReturnNull()
        {
            string expected = null;
            InjectionParameter parameter = new InjectionParameter(typeof(string), expected);
            AssertExpectedValue(parameter, typeof(string), expected);
        }

        [Fact]
        public void DependencyParameterCreatesExpectedResolver()
        {
            Type expectedType = typeof(ILogger);

            ResolvedParameter parameter = new ResolvedParameter<ILogger>();
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            AssertExtensions.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.Equal(expectedType, ((NamedTypeDependencyResolverPolicy)resolver).Type);
            Assert.Null(((NamedTypeDependencyResolverPolicy)resolver).Name);
        }

        [Fact]
        public void ResolvedParameterHandledNamedTypes()
        {
            Type expectedType = typeof(ILogger);
            string name = "special";

            ResolvedParameter parameter = new ResolvedParameter(expectedType, name);
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            AssertExtensions.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.Equal(expectedType, ((NamedTypeDependencyResolverPolicy)resolver).Type);
            Assert.Equal(name, ((NamedTypeDependencyResolverPolicy)resolver).Name);
        }

        [Fact]
        public void TypesImplicitlyConvertToResolvedDependencies()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof(int));

            Assert.Equal(1, values.Count);
            AssertExtensions.IsInstanceOfType(values[0], typeof(ResolvedParameter));
        }

        [Fact]
        public void ObjectsConverterToInjectionParametersResolveCorrectly()
        {
            List<InjectionParameterValue> values = GetParameterValues(15);

            InjectionParameter parameter = (InjectionParameter)values[0];
            Assert.Equal(typeof(int), parameter.ParameterType);
            IDependencyResolverPolicy policy = parameter.GetResolverPolicy(null);
            int result = (int)policy.Resolve(null);

            Assert.Equal(15, result);
        }

        [Fact]
        public void TypesAndObjectsImplicitlyConvertToInjectionParameters()
        {
            List<InjectionParameterValue> values = GetParameterValues(
                15, typeof(string), 22.5);

            Assert.Equal(3, values.Count);
            AssertExtensions.IsInstanceOfType(values[0], typeof(InjectionParameter));
            AssertExtensions.IsInstanceOfType(values[1], typeof(ResolvedParameter));
            AssertExtensions.IsInstanceOfType(values[2], typeof(InjectionParameter));
        }

        [Fact]
        public void ConcreteTypesMatch()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof(int), typeof(string), typeof(User));
            Type[] expectedTypes = Sequence.Collect(typeof(int), typeof(string), typeof(User));
            for (int i = 0; i < values.Count; ++i)
            {
                Assert.True(values[i].MatchesType(expectedTypes[i]));
            }
        }

        [Fact]
        public void CreatingInjectionParameterWithNullValueThrows()
        {
            AssertExtensions.AssertException<ArgumentNullException>(() =>
                {
                    new InjectionParameter(null);
                });
        }

        [Fact]
        public void InjectionParameterForNullValueReturnsExpectedValueIfTypeIsSuppliedExplicitly()
        {
            var parameter = new InjectionParameter(typeof(string), null);

            AssertExpectedValue(parameter, typeof(string), null);
        }

        private void AssertExpectedValue(InjectionParameter parameter, Type expectedType, object expectedValue)
        {
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);
            object result = resolver.Resolve(null);

            Assert.Equal(expectedType, parameter.ParameterType);
            AssertExtensions.IsInstanceOfType(resolver, typeof(LiteralValueDependencyResolverPolicy));
            Assert.Equal(expectedValue, result);
        }

        private List<InjectionParameterValue> GetParameterValues(params object[] values)
        {
            return InjectionParameterValue.ToParameters(values).ToList();
        }
    }
}
