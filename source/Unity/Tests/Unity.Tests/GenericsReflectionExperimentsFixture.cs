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
using Xunit;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Experimenting with reflection and generics.
    /// </summary>
     
    public class GenericsReflectionExperimentsFixture
    {
        // Experiments learning about reflection and generics
        [Fact]
        public void ConcreteGenericTypes_ReturnConstructorThatTakeGenericsInReflection()
        {
            Type t = typeof(LoggingCommand<User>);
            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors.Where(
                c => c.GetParameters()[0].ParameterType == typeof(ICommand<User>)).First();

            Assert.NotNull(ctor);
        }

        [Fact]
        public void OpenGenericTypes_GenericPropertiesAreReturnedByReflection()
        {
            Type t = typeof(LoggingCommand<>);
            PropertyInfo[] props = t.GetTypeInfo().DeclaredProperties.ToArray();
            Assert.Equal(1, props.Length);
        }

        [Fact]
        public void GivenGenericConstructorParameters_CanGetConcreteConstructor()
        {
            Type openType = typeof(Pathological<,>);
            Type targetType = typeof(Pathological<User, Account>);

            Assert.True(openType.GetTypeInfo().ContainsGenericParameters);
            Assert.False(targetType.GetTypeInfo().ContainsGenericParameters);

            ConstructorInfo ctor = targetType.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Count() == 2)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ICommand<>) &&
                    c.GetParameters()[0].ParameterType == typeof(ICommand<>))
                .FirstOrDefault();
            Assert.Null(ctor);

            ConstructorInfo concreteCtor =
                targetType.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Count() == 2)
                .Where(c => c.GetParameters()[0].ParameterType == typeof(ICommand<Account>) &&
                    c.GetParameters()[1].ParameterType == typeof(ICommand<User>))
                .FirstOrDefault();
            Assert.NotNull(concreteCtor);

            ConstructorInfo[] openCtors = openType.GetTypeInfo().DeclaredConstructors.ToArray();
            Assert.Equal(1, openCtors.Length);

            ParameterInfo[] ctorParams = openCtors[0].GetParameters();
            Assert.Equal(2, ctorParams.Length);
            Assert.True(ctorParams[0].ParameterType.GetTypeInfo().ContainsGenericParameters);
            Assert.Same(typeof(ICommand<>), ctorParams[0].ParameterType.GetGenericTypeDefinition());

            Type[] openTypeArgs = openType.GetTypeInfo().GenericTypeParameters;
            Type[] ctorParamArgs = ctorParams[0].ParameterType.GenericTypeArguments;
            Assert.Same(openTypeArgs[1], ctorParamArgs[0]);
        }

        [Fact]
        public void CanFigureOutOpenTypeDefinitionsForParameters()
        {
            Type openType = typeof(Pathological<,>);

            ConstructorInfo ctor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            ParameterInfo param0 = ctor.GetParameters()[0];

            Assert.NotEqual(typeof(ICommand<>), param0.ParameterType);
            Assert.Equal(typeof(ICommand<>), param0.ParameterType.GetGenericTypeDefinition());

            Assert.False(param0.ParameterType.GetTypeInfo().IsGenericTypeDefinition);
            Assert.True(param0.ParameterType.GetTypeInfo().IsGenericType);
            Assert.True(typeof(ICommand<>).GetTypeInfo().IsGenericTypeDefinition);
            Assert.Equal(typeof(ICommand<>), typeof(ICommand<>).GetGenericTypeDefinition());
        }

        [Fact]
        public void CanDistinguishOpenAndClosedGenerics()
        {
            Type closed = typeof(ICommand<Account>);
            Assert.True(closed.GetTypeInfo().IsGenericType);
            Assert.False(closed.GetTypeInfo().ContainsGenericParameters);

            Type open = typeof(ICommand<>);
            Assert.True(open.GetTypeInfo().IsGenericType);
            Assert.True(open.GetTypeInfo().ContainsGenericParameters);
        }

        [Fact]
        public void CanFindClosedConstructorFromOpenConstructorInfo()
        {
            Type openType = typeof(Pathological<,>);
            Type closedType = typeof(Pathological<User, Account>);

            ConstructorInfo openCtor = openType.GetTypeInfo().DeclaredConstructors.ElementAt(0);
            Assert.Same(openCtor.DeclaringType, openType);
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

            Assert.Same(closedType, createdClosedType);

            ConstructorInfo closedCtor =
                closedType.GetMatchingConstructor(Types(typeof(ICommand<Account>), typeof(ICommand<User>)));

            Assert.Same(closedCtor, targetCtor);
        }

        [Fact]
        public void ConstructorHasGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<>).GetTypeInfo().DeclaredConstructors.ElementAt(0);
            Assert.True(HasOpenGenericParameters(ctor));
        }

        [Fact]
        public void ConstructorDoesNotHaveGenericArguments()
        {
            ConstructorInfo ctor = typeof(LoggingCommand<Account>).GetMatchingConstructor(Types(typeof(ICommand<Account>)));
            Assert.False(HasOpenGenericParameters(ctor));
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
