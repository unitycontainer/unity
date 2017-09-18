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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Tests.TestObjects;
using ObjectBuilder2;
using Unity.ObjectBuilder;

namespace Microsoft.Practices.Unity.Tests
{
    // Tests for the DependencyValue class and its derivatives
    [TestClass]
    public class InjectionParameterValueFixture
    {
        [TestMethod]
        public void InjectionParameterReturnsExpectedValue()
        {
            int expected = 12;
            InjectionParameter parameter = new InjectionParameter(expected);
            AssertExpectedValue(parameter, typeof(int), expected);
        }

        [TestMethod]
        public void InjectionParameterCanTakeExplicitType()
        {
            double expected = Math.E;
            InjectionParameter parameter = new InjectionParameter<double>(expected);
            AssertExpectedValue(parameter, typeof(double), expected);
        }

        [TestMethod]
        public void InjectionParameterCanReturnNull()
        {
            string expected = null;
            InjectionParameter parameter = new InjectionParameter(typeof(string), expected);
            AssertExpectedValue(parameter, typeof(string), expected);
        }

        [TestMethod]
        public void DependencyParameterCreatesExpectedResolver()
        {
            Type expectedType = typeof(ILogger);

            ResolvedParameter parameter = new ResolvedParameter<ILogger>();
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            Assert.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.AreEqual(expectedType, ( (NamedTypeDependencyResolverPolicy)resolver ).Type);
            Assert.IsNull(( (NamedTypeDependencyResolverPolicy)resolver ).Name);
        }

        [TestMethod]
        public void ResolvedParameterHandledNamedTypes()
        {
            Type expectedType = typeof(ILogger);
            string name = "special";

            ResolvedParameter parameter = new ResolvedParameter(expectedType, name);
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);

            Assert.IsInstanceOfType(resolver, typeof(NamedTypeDependencyResolverPolicy));
            Assert.AreEqual(expectedType, ( (NamedTypeDependencyResolverPolicy)resolver ).Type);
            Assert.AreEqual(name, ( (NamedTypeDependencyResolverPolicy)resolver ).Name);
        }

        [TestMethod]
        public void TypesImplicitlyConvertToResolvedDependencies()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof(int));

            Assert.AreEqual(1, values.Count);
            Assert.IsInstanceOfType(values[0], typeof(ResolvedParameter));
        }

        [TestMethod]
        public void ObjectsConverterToInjectionParametersResolveCorrectly()
        {
            List<InjectionParameterValue> values = GetParameterValues(15);

            InjectionParameter parameter = (InjectionParameter)values[0];
            Assert.AreEqual(typeof(int), parameter.ParameterType);
            IDependencyResolverPolicy policy = parameter.GetResolverPolicy(null);
            int result = (int)policy.Resolve(null);

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void TypesAndObjectsImplicitlyConvertToInjectionParameters()
        {
            List <InjectionParameterValue> values = GetParameterValues(
                15, typeof(string), 22.5);

            Assert.AreEqual(3, values.Count);
            Assert.IsInstanceOfType(values[0], typeof(InjectionParameter));
            Assert.IsInstanceOfType(values[1], typeof(ResolvedParameter));
            Assert.IsInstanceOfType(values[2], typeof(InjectionParameter));
        }

        [TestMethod]
        public void ConcreteTypesMatch()
        {
            List<InjectionParameterValue> values = GetParameterValues(typeof (int), typeof (string), typeof (User));
            Type[] expectedTypes = Sequence.Collect(typeof (int), typeof (string), typeof (User));
            for(int i = 0; i < values.Count; ++i)
            {
                Assert.IsTrue(values[i].MatchesType(expectedTypes[i]));
            }
        }

        private void AssertExpectedValue(InjectionParameter parameter, Type expectedType, object expectedValue)
        {
            IDependencyResolverPolicy resolver = parameter.GetResolverPolicy(expectedType);
            object result = resolver.Resolve(null);

            Assert.AreEqual(expectedType, parameter.ParameterType);
            Assert.IsInstanceOfType(resolver, typeof(LiteralValueDependencyResolverPolicy));
            Assert.AreEqual(expectedValue, result);
        }

        private List<InjectionParameterValue> GetParameterValues(params object[] values)
        {
            return InjectionParameterValue.ToParameters(values).ToList();
        }
    }
}
