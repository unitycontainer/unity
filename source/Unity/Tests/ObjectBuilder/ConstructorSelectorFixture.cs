// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using ObjectBuilder2.Tests.TestDoubles;
using ObjectBuilder2.Tests.TestObjects;
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

namespace ObjectBuilder2.Tests
{
    /// <summary>
    /// Tests for the default ConstructorSelectorPolicy
    /// </summary>
     
    public class ConstructorSelectorFixture
    {
        [Fact]
        public void SelectorPicksDefaultConstructor()
        {
            IConstructorSelectorPolicy policy = CreateSelector();
            ConstructorInfo expectedCtor = GetConstructor<object>();
            MockBuilderContext context = GetContext<object>();
            SelectedConstructor result = policy.SelectConstructor(context, new PolicyList());

            Assert.Equal(expectedCtor, result.Constructor);
        }

        [Fact]
        public void SelectorPicksConstructorWithAttribute()
        {
            IConstructorSelectorPolicy policy = CreateSelector();
            ConstructorInfo expected = GetConstructor<ObjectWithMarkedConstructor>(typeof(string));

            SelectedConstructor result = policy.SelectConstructor(GetContext<ObjectWithMarkedConstructor>(), new PolicyList());

            Assert.Equal(expected, result.Constructor);
        }

        [Fact]
        public void SelectorPicksLongestConstructor()
        {
            IConstructorSelectorPolicy policy = CreateSelector();
            ConstructorInfo expected = GetConstructor<ObjectWithMultipleConstructors>(
                typeof(int), typeof(string));

            SelectedConstructor result =
                policy.SelectConstructor(GetContext<ObjectWithMultipleConstructors>(), new PolicyList());

            Assert.Equal(expected, result.Constructor);
        }

        [Fact]
        public void SelectorThrowsIfConstructorsAreAmbiguous()
        {
            IConstructorSelectorPolicy policy = CreateSelector();

            try
            {
                policy.SelectConstructor(GetContext<ObjectWithAmbiguousConstructors>(), new PolicyList());
            }
            catch (InvalidOperationException)
            {
                // If we got here we're ok
                return;
            }
            Assert.True(false, string.Format("Expected exception did not occur"));
        }

        [Fact]
        public void SelectorPicksMarkedConstructorEvenIfOtherwiseAmbiguous()
        {
            IConstructorSelectorPolicy policy = CreateSelector();
            ConstructorInfo expected = GetConstructor<ObjectWithAmbiguousMarkedConstructor>(
                typeof(string), typeof(string), typeof(int));

            SelectedConstructor result =
                policy.SelectConstructor(GetContext<ObjectWithAmbiguousMarkedConstructor>(), new PolicyList());

            Assert.Equal(expected, result.Constructor);
        }

        private static ConstructorSelectorPolicy<InjectionConstructorAttribute> CreateSelector()
        {
            return new ConstructorSelectorPolicy<InjectionConstructorAttribute>();
        }

        private static ConstructorInfo GetConstructor<T>(params Type[] paramTypes)
        {
            return typeof(T).GetTypeInfo().DeclaredConstructors.First(c => ParameterTypesMatch(c.GetParameters(), paramTypes));
        }

        private static bool ParameterTypesMatch(ParameterInfo[] parameters, Type[] paramTypesToMatch)
        {
            if (parameters.Length != paramTypesToMatch.Length)
            {
                return false;
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != paramTypesToMatch[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static MockBuilderContext GetContext<T>()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.BuildKey = new NamedTypeBuildKey<T>();
            return context;
        }
    }
}
