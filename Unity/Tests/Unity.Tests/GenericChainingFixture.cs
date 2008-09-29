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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    // Test fixture to verify generic object chaining.
    // Reported as a bug in http://www.codeplex.com/unity/Thread/View.aspx?ThreadId=27231
    [TestClass]
    public class GenericChainingFixture
    {
        [TestMethod]
        public void CanSpecializeGenericTypes()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>));
            ICommand<User> cmd = container.Resolve<ICommand<User>>();
            Assert.IsInstanceOfType(cmd, typeof(ConcreteCommand<User>));
        }

        [TestMethod]
        public void ConfiguringConstructorThatTakesOpenGenericTypeDoesNotThrow()
        {
            IUnityContainer container = new UnityContainer();
            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")));
        }

        [TestMethod]
        public void CanChainGenericTypes()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete");

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")));

            ICommand<User> cmd = container.Resolve<ICommand<User>>();
            LoggingCommand<User> logCmd = (LoggingCommand<User>)cmd;

            Assert.IsNotNull(logCmd.Inner);
            Assert.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<User>));
        }

        [TestMethod]
        public void CanChainGenericTypesViaRegisterTypeMethod()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete");

            ICommand<User> cmd = container.Resolve<ICommand<User>>();
            LoggingCommand<User> logCmd = (LoggingCommand<User>)cmd;

            Assert.IsNotNull(logCmd.Inner);
            Assert.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<User>));
        }

        [TestMethod]
        public void CanConfigureGenericMethodInjectionInContainer()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")),
                    new InjectionMethod("ChainedExecute", new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete")
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");
        }

        [TestMethod]
        public void ConfiguredGenericMethodInjectionIsCalled()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")),
                    new InjectionMethod("ChainedExecute", new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete")
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            ICommand<Account> result = container.Resolve<ICommand<Account>>();
            LoggingCommand<Account> lc = (LoggingCommand<Account>)result;

            Assert.IsTrue(lc.ChainedExecuteWasCalled);
        }

        [TestMethod]
        public void CanConfigureInjectionForNonGenericMethodOnGenericClass()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>));
            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                                       new InjectionConstructor(),
                                       new InjectionMethod("InjectMe"));

            ICommand<Account> result = container.Resolve<ICommand<Account>>();
            LoggingCommand<Account> logResult = (LoggingCommand<Account>)result;

            Assert.IsTrue(logResult.WasInjected);
        }

        [TestMethod]
        public void CanCallDefaultConstructorOnGeneric()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                    new InjectionConstructor());

            ICommand<User> result = container.Resolve<ICommand<User>>();
            Assert.IsInstanceOfType(result, typeof(LoggingCommand<User>));

            ICommand<Account> accountResult = container.Resolve<ICommand<Account>>();

            Assert.IsInstanceOfType(accountResult, typeof(LoggingCommand<Account>));

        }

        [TestMethod]
        public void CanConfigureInjectionForGenericProperty()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                    new InjectionConstructor(),
                    new InjectionProperty("Inner",
                        new ResolvedParameter(typeof(ICommand<>), "inner")));
        }

        [TestMethod]
        public void GenericPropertyIsActuallyInjected()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                    new InjectionConstructor(),
                    new InjectionProperty("Inner",
                        new ResolvedParameter(typeof(ICommand<>), "inner")));

            ICommand<Account> result = container.Resolve<ICommand<Account>>();

            LoggingCommand<Account> actualResult = (LoggingCommand<Account>)result;

            Assert.IsNotNull(actualResult.Inner);
            Assert.IsInstanceOfType(actualResult.inner, typeof(ConcreteCommand<Account>));
        }

        [TestMethod]
        public void CanInjectNonGenericPropertyOnGenericClass()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>));

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(ConcreteCommand<>),
                                       new InjectionProperty("NonGenericProperty"));

            ConcreteCommand<User> result = (ConcreteCommand<User>)(container.Resolve<ICommand<User>>());
            Assert.IsNotNull(result.NonGenericProperty);
        }

        [TestMethod]
        public void CanInjectNestedGenerics()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete");

            container.Configure<InjectedMembers>()
                .ConfigureInjectionFor(typeof(LoggingCommand<>),
                new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")));

            ICommand<Nullable<Customer>> cmd = container.Resolve<ICommand<Nullable<Customer>>>();
            LoggingCommand<Nullable<Customer>> logCmd = (LoggingCommand<Nullable<Customer>>)cmd;

            Assert.IsNotNull(logCmd.Inner);
            Assert.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<Nullable<Customer>>));

        }


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

    // Our generic interface 
    interface ICommand<T>
    {
        void Execute(T data);
        void ChainedExecute(ICommand<T> inner);
    }

    // An implementation of ICommand that executes them.
    class ConcreteCommand<T> : ICommand<T>
    {
        private object p = null;

        public void Execute(T data)
        {
        }

        public void ChainedExecute(ICommand<T> inner)
        {

        }

        public object NonGenericProperty
        {
            get { return p; }
            set { p = value; }
        }
    }

    // And a decorator implementation that wraps an Inner ICommand<>
    class LoggingCommand<T> : ICommand<T>
    {
        public ICommand<T> inner;

        public bool ChainedExecuteWasCalled = false;
        public bool WasInjected = false;


        public LoggingCommand(ICommand<T> inner)
        {
            this.inner = inner;
        }

        public LoggingCommand()
        {

        }

        public ICommand<T> Inner
        {
            get { return inner; }
            set { inner = value; }
        }

        public void Execute(T data)
        {
            // do logging here
            Inner.Execute(data);
        }

        public void ChainedExecute(ICommand<T> innerCommand)
        {
            ChainedExecuteWasCalled = true;
        }

        public void InjectMe()
        {
            WasInjected = true;
        }
    }

    // A type with some nasty generics in the constructor
    class Pathological<T1, T2>
    {
        public Pathological(ICommand<T2> cmd1, ICommand<T1> cmd2)
        {
        }

        public ICommand<T2> AProperty
        {
            get { return null; }
            set { }
        }
    }

    // A couple of sample objects we're stuffing into our commands
    class User
    {
        public void DoSomething(string message)
        {

        }
    }

    class Account
    {

    }

    // Value type used for testing nesting
    struct Customer
    {
        
    }
}
