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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Experimenting with reflection and generics.
    /// </summary>
    [TestClass]
    public class GenericsReflectionExperimentsFixture
    {
        //
        // Experiments learning about reflection and generics
        //

        [TestMethod]
        public void ConcreteGenericTypes_ReturnConstructorThatTakeGenericsInReflection()
        {
            Type t = typeof(LoggingCommand<User>);
            ConstructorInfo ctor = t.GetConstructor(
                new Type[] { typeof(ICommand<User>) });

            Assert.IsNotNull(ctor);
        }

        [TestMethod]
        public void OpenGenericTypes_GenericPropertiesAreReturnedByReflection()
        {
            Type t = typeof(LoggingCommand<>);
            PropertyInfo[] props = t.GetProperties();
            Assert.AreEqual(1, props.Length);
        }



        [TestMethod]
        public void GivenGenericConstructorParameters_CanGetConcreteConstructor()
        {
            Type openType = typeof(Pathological<,>);
            Type targetType = typeof(Pathological<User, Account>);

            Assert.IsTrue(openType.ContainsGenericParameters);
            Assert.IsFalse(targetType.ContainsGenericParameters);

            ConstructorInfo ctor = targetType.GetConstructor(new Type[] { typeof(ICommand<>), typeof(ICommand<>) });
            Assert.IsNull(ctor);
            ConstructorInfo concreteCtor =
                targetType.GetConstructor(new Type[] { typeof(ICommand<Account>), typeof(ICommand<User>) });
            Assert.IsNotNull(concreteCtor);

            ConstructorInfo[] openCtors = openType.GetConstructors();
            Assert.AreEqual(1, openCtors.Length);

            ParameterInfo[] ctorParams = openCtors[0].GetParameters();
            Assert.AreEqual(2, ctorParams.Length);
            Assert.IsTrue(ctorParams[0].ParameterType.ContainsGenericParameters);
            Assert.AreSame(typeof(ICommand<>), ctorParams[0].ParameterType.GetGenericTypeDefinition());

            Type[] openTypeArgs = openType.GetGenericArguments();
            Type[] ctorParamArgs = ctorParams[0].ParameterType.GetGenericArguments();
            Assert.AreSame(openTypeArgs[1], ctorParamArgs[0]);
        }

        [TestMethod]
        public void CanFigureOutOpenTypeDefinitionsForParameters()
        {
            Type openType = typeof(Pathological<,>);

            ConstructorInfo ctor = openType.GetConstructors()[0];
            ParameterInfo param0 = ctor.GetParameters()[0];

            Assert.AreNotEqual(typeof(ICommand<>), param0.ParameterType);
            Assert.AreEqual(typeof(ICommand<>), param0.ParameterType.GetGenericTypeDefinition());

            Assert.IsFalse(param0.ParameterType.IsGenericTypeDefinition);
            Assert.IsTrue(param0.ParameterType.IsGenericType);
            Assert.IsTrue(typeof(ICommand<>).IsGenericTypeDefinition);
            Assert.AreEqual(typeof(ICommand<>), typeof(ICommand<>).GetGenericTypeDefinition());
        }

        [TestMethod]
        public void CanDistinguishOpenAndClosedGenerics()
        {
            Type closed = typeof(ICommand<Account>);
            Assert.IsTrue(closed.IsGenericType);
            Assert.IsFalse(closed.ContainsGenericParameters);

            Type open = typeof(ICommand<>);
            Assert.IsTrue(open.IsGenericType);
            Assert.IsTrue(open.ContainsGenericParameters);

        }

        [TestMethod]
        public void CanFindClosedConstructorFromOpenConstructorInfo()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<User, Account>);

            ConstructorInfo openCtor = openType.GetConstructors()[0];
            Assert.AreSame(openCtor.DeclaringType, openType);
            Type createdClosedType = openType.MakeGenericType(closedType.GetGenericArguments());

            // Go through the parameter list of the open constructor and fill in the
            // type arguments for generic parameters based on the arguments used to
            // create the closed types.

            Type[] closedTypeParams = closedType.GetGenericArguments();

            List<Type> closedCtorParamTypes = new List<Type>();

            List<int> parameterPositions = new List<int>();
            foreach (ParameterInfo openParam in openCtor.GetParameters())
            {
                closedCtorParamTypes.Add(ClosedTypeFromOpenParameter(openParam, closedTypeParams));

                Type[] genericParameters = openParam.ParameterType.GetGenericArguments();
                foreach (Type gp in genericParameters)
                {
                    parameterPositions.Add(gp.GenericParameterPosition);
                }

            }

            CollectionAssert.AreEqual(new int[] { 1, 0 }, parameterPositions);

            ConstructorInfo targetCtor = closedType.GetConstructor(closedCtorParamTypes.ToArray());


            Assert.AreSame(closedType, createdClosedType);

            ConstructorInfo closedCtor =
                closedType.GetConstructor(Types(typeof(ICommand<Account>), typeof(ICommand<User>)));

            Assert.AreSame(closedCtor, targetCtor);

        }

        [TestMethod]
        public void ConstructorHasGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetConstructors()[0];
            Assert.IsTrue(HasOpenGenericParameters(ctor));
        }

        [TestMethod]
        public void ConstructorDoesNotHaveGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<Account>).GetConstructor(Types(typeof(ICommand<Account>)));
            Assert.IsFalse(HasOpenGenericParameters(ctor));
        }

        private Type ClosedTypeFromOpenParameter(ParameterInfo openGenericParameter, Type[] typeParams)
        {
            Type[] genericParameters = openGenericParameter.ParameterType.GetGenericArguments();
            Type[] genericTypeParams = new Type[genericParameters.Length];
            for (int i = 0; i < genericParameters.Length; ++i)
            {
                genericTypeParams[i] = typeParams[genericParameters[i].GenericParameterPosition];
            }
            return openGenericParameter.ParameterType.GetGenericTypeDefinition().MakeGenericType(genericTypeParams);
        }

        private bool HasOpenGenericParameters(ConstructorInfo ctor)
        {
            foreach (ParameterInfo param in ctor.GetParameters())
            {
                if (param.ParameterType.IsGenericType &&
                    param.ParameterType.ContainsGenericParameters)
                {
                    return true;
                }
            }
            return false;
        }

        private static Type[] Types(params Type[] t)
        {
            return t;
        }
    }
}
