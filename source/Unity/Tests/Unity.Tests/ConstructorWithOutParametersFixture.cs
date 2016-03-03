// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
     
    public class ConstructorWithOutAndRefParametersFixture
    {
        [Fact]
        public void CanBuildUpExistingObjectOnTypeWithCtorWithRefParameter()
        {
            IUnityContainer container =
                new UnityContainer()
                    .RegisterType<TypeWithConstructorWithRefParameter>(new InjectionProperty("Property", 10));
            string ignored = "ignored";
            TypeWithConstructorWithRefParameter instance = new TypeWithConstructorWithRefParameter(ref ignored);

            container.BuildUp(instance);

            Assert.Equal(10, instance.Property);
        }

        [Fact]
        public void CanBuildUpExistingObjectOnTypeWithCtorWithOutParameter()
        {
            IUnityContainer container =
                new UnityContainer()
                    .RegisterType<TypeWithConstructorWithOutParameter>(new InjectionProperty("Property", 10));
            string ignored = "ignored";
            TypeWithConstructorWithOutParameter instance = new TypeWithConstructorWithOutParameter(out ignored);

            container.BuildUp(instance);

            Assert.Equal(10, instance.Property);
        }

        [Fact]
        public void ResolvingANewInstanceOfTypeWithCtorWithRefParameterThrows()
        {
            IUnityContainer container = new UnityContainer();

            try
            {
                TypeWithConstructorWithRefParameter instance = container.Resolve<TypeWithConstructorWithRefParameter>();
                Assert.True(false, string.Format("should have thrown"));
            }
            catch (ResolutionFailedException)
            {
                // expected
            }
        }

        [Fact]
        public void ResolvingANewInstanceOfTypeWithCtorWithOutParameterThrows()
        {
            IUnityContainer container = new UnityContainer();

            try
            {
                TypeWithConstructorWithOutParameter instance = container.Resolve<TypeWithConstructorWithOutParameter>();
                Assert.True(false, string.Format("should have thrown"));
            }
            catch (ResolutionFailedException)
            {
                // expected
            }
        }

        public class TypeWithConstructorWithRefParameter
        {
            public TypeWithConstructorWithRefParameter(ref string ignored)
            {
            }

            public int Property { get; set; }
        }

        public class TypeWithConstructorWithOutParameter
        {
            public TypeWithConstructorWithOutParameter(out string ignored)
            {
                ignored = null;
            }

            public int Property { get; set; }
        }
    }
}
