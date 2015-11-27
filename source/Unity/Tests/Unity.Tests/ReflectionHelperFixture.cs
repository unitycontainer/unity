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
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
{
    [TestClass]
    public class ReflectionHelperFixture
    {
        [TestMethod]
        public void ShouldDetermineStringIsNotGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(string));
            Assert.IsFalse(helper.IsGenericType);
        }

        [TestMethod]
        public void ShouldDetermineICommandIsGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(ICommand<User>));
            Assert.IsTrue(helper.IsGenericType);
        }

        [TestMethod]
        public void ShouldIdentifyOpenGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(ICommand<>));
            Assert.IsTrue(helper.IsOpenGeneric);
        }

        [TestMethod]
        public void IntIsNotAnOpenGeneric()
        {
            ReflectionHelper helper = new ReflectionHelper(typeof(int));
            Assert.IsFalse(helper.IsOpenGeneric);
        }

        [TestMethod]
        public void RegularMethodHasNoOpenParameters()
        {
            MethodInfo executeMethod = typeof(LoggingCommand<User>).GetTypeInfo().GetDeclaredMethod("Execute");
            Assert.IsFalse(ReflectionHelper.MethodHasOpenGenericParameters(executeMethod));
        }

        [TestMethod]
        public void ParameterReflectionHelperDetectsOpenGenericParameters()
        {
            MethodBase executeMethod = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterInfo[] methodParams = executeMethod.GetParameters();

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);

            Assert.IsTrue(helper.IsOpenGeneric);
        }

        [TestMethod]
        public void ClosingParameterTypesReturnsOriginalTypeWhenItIsClosed()
        {
            MethodBase m = typeof(User).GetTypeInfo().GetDeclaredMethod("DoSomething");
            ParameterInfo[] methodParams = m.GetParameters();

            Assert.AreEqual(1, methodParams.Length);

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);
            Assert.AreSame(typeof(string), helper.GetClosedParameterType(new Type[0]));
        }

        [TestMethod]
        public void CanCreateClosedParameterTypeFromOpenOne()
        {
            MethodBase m = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterReflectionHelper helper = new ParameterReflectionHelper(
                m.GetParameters()[0]);
            Type closedType = helper.GetClosedParameterType(typeof(LoggingCommand<User>).GenericTypeArguments);
            Assert.AreSame(typeof(ICommand<User>), closedType);
        }

        [TestMethod]
        public void CanGetIfAnyParametersAreOpenGeneric()
        {
            Type openType = typeof(Pathological<,>);
            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);

            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);
            Assert.IsTrue(helper.MethodHasOpenGenericParameters);
        }

        [TestMethod]
        public void CanGetClosedParameterTypesFromOpenOnes()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<Account, User>);
            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);

            Type[] closedTypes = helper.GetClosedParameterTypes(closedType.GenericTypeArguments);

            closedTypes.AssertContainsExactly(typeof(ICommand<User>), typeof(ICommand<Account>));
        }

        [TestMethod]
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
