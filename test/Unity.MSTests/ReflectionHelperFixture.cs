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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Utility;
using ObjectBuilder2;

namespace Microsoft.Practices.Unity.Tests
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
            MethodInfo executeMethod = typeof(LoggingCommand<User>).GetMethod("Execute");
            Assert.IsFalse(ReflectionHelper.MethodHasOpenGenericParameters(executeMethod));
        }

        [TestMethod]
        public void ParameterReflectionHelperDetectsOpenGenericParameters()
        {
            MethodBase executeMethod = typeof(LoggingCommand<>).GetConstructors()[0];
            ParameterInfo[] methodParams = executeMethod.GetParameters();

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);

            Assert.IsTrue(helper.IsOpenGeneric);
        }

        [TestMethod]
        public void ClosingParameterTypesReturnsOriginalTypeWhenItIsClosed()
        {
            MethodBase m = typeof(User).GetMethod("DoSomething");
            ParameterInfo[] methodParams = m.GetParameters();

            Assert.AreEqual(1, methodParams.Length);

            ParameterReflectionHelper helper = new ParameterReflectionHelper(methodParams[0]);
            Assert.AreSame(typeof(string), helper.GetClosedParameterType(new Type[0]));
        }

        [TestMethod]
        public void CanCreateClosedParameterTypeFromOpenOne()
        {
            MethodBase m = typeof(LoggingCommand<>).GetConstructors()[0];
            ParameterReflectionHelper helper = new ParameterReflectionHelper(
                m.GetParameters()[0]);
            Type closedType = helper.GetClosedParameterType(typeof(LoggingCommand<User>).GetGenericArguments());
            Assert.AreSame(typeof(ICommand<User>), closedType);
        }

        [TestMethod]
        public void CanGetIfAnyParametersAreOpenGeneric()
        {
            Type openType = typeof(Pathological<,>);
            ConstructorInfo ctor = openType.GetConstructors()[0];

            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);
            Assert.IsTrue(helper.MethodHasOpenGenericParameters);
        }

        [TestMethod]
        public void CanGetClosedParameterTypesFromOpenOnes()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<Account, User>);
            ConstructorInfo ctor = openType.GetConstructors()[0];
            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);

            Type[] closedTypes = helper.GetClosedParameterTypes(closedType.GetGenericArguments());

            CollectionAssert.AreEqual(
                Sequence.Collect(typeof(ICommand<User>), typeof(ICommand<Account>)),
                closedTypes);
        }

        [TestMethod]
        public void CanGetClosedParameterArrayTypesFromOpenOnes()
        {
            Type openType = typeof(TypeWithArrayConstructorParameters<,>);
            Type closedType = typeof(TypeWithArrayConstructorParameters<Account, User>);
            ConstructorInfo ctor = openType.GetConstructors()[0];
            MethodReflectionHelper helper = new MethodReflectionHelper(ctor);

            Type[] closedTypes = helper.GetClosedParameterTypes(closedType.GetGenericArguments());

            CollectionAssert.AreEqual(
                Sequence.Collect(typeof(User[]), typeof(Account[,])),
                closedTypes);
        }

        public class TypeWithArrayConstructorParameters<T1, T2>
        {
            public TypeWithArrayConstructorParameters(T2[] param1, T1[,] param2)
            {
            }
        }
    }
}
