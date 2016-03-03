// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using ObjectBuilder2;
using Unity.TestSupport;
using Unity.Utility;
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
     
    public class ReflectionHelperFixture
    {
        [Fact]
        public void ShouldDetermineStringIsNotGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(string));
            Assert.False(helper.IsGenericType);
        }

        [Fact]
        public void ShouldDetermineICommandIsGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(ICommand<User>));
            Assert.True(helper.IsGenericType);
        }

        [Fact]
        public void ShouldIdentifyOpenGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(ICommand<>));
            Assert.True(helper.IsOpenGeneric);
        }

        [Fact]
        public void IntIsNotAnOpenGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(int));
            Assert.False(helper.IsOpenGeneric);
        }

        [Fact]
        public void RegularMethodHasNoOpenParameters()
        {
            MethodInfo executeMethod = typeof(LoggingCommand<User>).GetTypeInfo().GetDeclaredMethod("Execute");
            Assert.False(ReflectionHelper.MethodHasOpenGenericParameters(executeMethod));
        }

        [Fact]
        public void ParameterReflectionHelperDetectsOpenGenericParameters()
        {
            MethodBase executeMethod = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterInfo[] methodParams = executeMethod.GetParameters();

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);

            Assert.True(helper.IsOpenGeneric);
        }

        [Fact]
        public void ClosingParameterTypesReturnsOriginalTypeWhenItIsClosed()
        {
            MethodBase m = typeof(User).GetTypeInfo().GetDeclaredMethod("DoSomething");
            ParameterInfo[] methodParams = m.GetParameters();

            Assert.Equal(1, methodParams.Length);

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);
            Assert.Same(typeof(string), helper.GetClosedParameterType(new Type[0]));
        }

        [Fact]
        public void CanCreateClosedParameterTypeFromOpenOne()
        {
            MethodBase m = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterReflectionHelper helper = new ParameterReflectionHelper(
                m.GetParameters()[0]);
            Type closedType = helper.GetClosedParameterType(typeof(LoggingCommand<User>).GenericTypeArguments);
            Assert.Same(typeof(ICommand<User>), closedType);
        }

        [Fact]
        public void CanGetIfAnyParametersAreOpenGeneric()
        {
            Type openType = typeof(Pathological<,>);
            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);

            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);
            Assert.True(helper.MethodHasOpenGenericParameters);
        }

        [Fact]
        public void CanGetClosedParameterTypesFromOpenOnes()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<Account, User>);
            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);

            Type[] closedTypes = helper.GetClosedParameterTypes(closedType.GenericTypeArguments);

            closedTypes.AssertContainsExactly(typeof(ICommand<User>), typeof(ICommand<Account>));
        }

        [Fact]
        public void CanGetClosedParameterArrayTypesFromOpenOnes()
        {
            Type openType = typeof(TypeWithArrayConstructorParameters<,>);
            Type closedType = typeof(TypeWithArrayConstructorParameters<Account, User>);
            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);

            Type[] closedTypes = helper.GetClosedParameterTypes(closedType.GenericTypeArguments);

            closedTypes.AssertContainsExactly(typeof(User[]), typeof(Account[,]));
        }

        public class TypeWithArrayConstructorParameters<T1, T2>
        {
            public TypeWithArrayConstructorParameters(T2[] param1, T1[,] param2)
            {
            }
        }
    }
}
