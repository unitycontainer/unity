// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.TestSupport;
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
    /// <summary>
    /// Experimenting with reflection and generics.
    /// </summary>
    [TestClass]
    public class GenericsReflectionExperimentsFixture
    {
        // Experiments learning about reflection and generics
        [TestMethod]
        public void ConcreteGenericTypes_ReturnConstructorThatTakeGenericsInReflection()
        {
            Type t = typeof(LoggingCommand<User>);
            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors.Where(
                c => c.GetParameters()[0].ParameterType == typeof(ICommand<User>)).First();

            Assert.IsNotNull(ctor);
        }

        [TestMethod]
        public void OpenGenericTypes_GenericPropertiesAreReturnedByReflection()
        {
            Type t = typeof(LoggingCommand<>);
            PropertyInfo[] props = t.GetTypeInfo().DeclaredProperties.ToArray();
            Assert.AreEqual(1, props.Length);
        }

        [TestMethod]
        public void GivenGenericConstructorParameters_CanGetConcreteConstructor()
        {
            Type openType = typeof(Pathological<,>);
            Type targetType = typeof(Pathological<User, Account>);

            Assert.IsTrue(openType.GetTypeInfo().ContainsGenericParameters);
            Assert.IsFalse(targetType.GetTypeInfo().ContainsGenericParameters);

            ConstructorInfo ctor = targetType.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Count() == 2)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ICommand<>) &&
                    c.GetParameters()[0].ParameterType == typeof(ICommand<>))
                .FirstOrDefault();
            Assert.IsNull(ctor);

            ConstructorInfo concreteCtor =
                targetType.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Count() == 2)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ICommand<Account>) &&
                    c.GetParameters()[1].ParameterType == typeof(ICommand<User>))
                .FirstOrDefault();
            Assert.IsNotNull(concreteCtor);

            ConstructorInfo[] openCtors = openType.GetTypeInfo().DeclaredConstructors.ToArray();
            Assert.AreEqual(1, openCtors.Length);

            ParameterInfo[] ctorParams = openCtors[0].GetParameters();
            Assert.AreEqual(2, ctorParams.Length);
            Assert.IsTrue(ctorParams[0].ParameterType.GetTypeInfo().ContainsGenericParameters);
            Assert.AreSame(typeof(ICommand<>), ctorParams[0].ParameterType.GetGenericTypeDefinition());

            Type[] openTypeArgs = openType.GetTypeInfo().GenericTypeParameters;
            Type[] ctorParamArgs = ctorParams[0].ParameterType.GenericTypeArguments;
            Assert.AreSame(openTypeArgs[1], ctorParamArgs[0]);
        }

        [TestMethod]
        public void CanFigureOutOpenTypeDefinitionsForParameters()
        {
            Type openType = typeof(Pathological<,>);

            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterInfo param0 = ctor.GetParameters()[0];

            Assert.AreNotEqual(typeof(ICommand<>), param0.ParameterType);
            Assert.AreEqual(typeof(ICommand<>), param0.ParameterType.GetGenericTypeDefinition());

            Assert.IsFalse(param0.ParameterType.GetTypeInfo().IsGenericTypeDefinition);
            Assert.IsTrue(param0.ParameterType.GetTypeInfo().IsGenericType);
            Assert.IsTrue(typeof(ICommand<>).GetTypeInfo().IsGenericTypeDefinition);
            Assert.AreEqual(typeof(ICommand<>), typeof(ICommand<>).GetGenericTypeDefinition());
        }

        [TestMethod]
        public void CanDistinguishOpenAndClosedGenerics()
        {
            Type closed = typeof(ICommand<Account>);
            Assert.IsTrue(closed.GetTypeInfo().IsGenericType);
            Assert.IsFalse(closed.GetTypeInfo().ContainsGenericParameters);

            Type open = typeof(ICommand<>);
            Assert.IsTrue(open.GetTypeInfo().IsGenericType);
            Assert.IsTrue(open.GetTypeInfo().ContainsGenericParameters);
        }

        [TestMethod]
        public void CanFindClosedConstructorFromOpenConstructorInfo()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<User, Account>);

            ConstructorInfo openCtor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            Assert.AreSame(openCtor.DeclaringType, openType);
            Type createdClosedType = openType.MakeGenericType(closedType.GenericTypeArguments);

            // Go through the parameter list of the open constructor and fill in the
            // type arguments for generic parameters based on the arguments used to
            // create the closed types.

            Type[] closedTypeParams = closedType.GenericTypeArguments;

            List<Type> closedCtorParamTypes = new List<Type>();

            List<int> parameterPositions = new List<int>();
            foreach (ParameterInfo openParam in openCtor.GetParameters())
            {
                closedCtorParamTypes.Add(ClosedTypeFromOpenParameter(openParam, closedTypeParams));

                Type[] genericParameters = openParam.ParameterType.GenericTypeArguments;
                foreach (Type gp in genericParameters)
                {
                    parameterPositions.Add(gp.GenericParameterPosition);
                }
            }

            CollectionAssertExtensions.AreEqual(new int[] { 1, 0 }, parameterPositions);

            ConstructorInfo targetCtor = closedType.GetMatchingConstructor(closedCtorParamTypes.ToArray());

            Assert.AreSame(closedType, createdClosedType);

            ConstructorInfo closedCtor =
                closedType.GetMatchingConstructor(Types(typeof(ICommand<Account>), typeof(ICommand<User>)));

            Assert.AreSame(closedCtor, targetCtor);
        }

        [TestMethod]
        public void ConstructorHasGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            Assert.IsTrue(HasOpenGenericParameters(ctor));
        }

        [TestMethod]
        public void ConstructorDoesNotHaveGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<Account>).GetMatchingConstructor(Types(typeof(ICommand<Account>)));
            Assert.IsFalse(HasOpenGenericParameters(ctor));
        }

        private Type ClosedTypeFromOpenParameter(ParameterInfo openGenericParameter, Type[] typeParams)
        {
            Type[] genericParameters = openGenericParameter.ParameterType.GenericTypeArguments;
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
                if (param.ParameterType.GetTypeInfo().IsGenericType &&
                    param.ParameterType.GetTypeInfo().ContainsGenericParameters)
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
