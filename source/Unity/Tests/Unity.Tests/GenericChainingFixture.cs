// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
            AssertExtensions.IsInstanceOfType(cmd, typeof(ConcreteCommand<User>));
        }

        [TestMethod]
        public void ConfiguringConstructorThatTakesOpenGenericTypeDoesNotThrow()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")));
        }

        [TestMethod]
        public void CanChainGenericTypes()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete");

            ICommand<User> cmd = container.Resolve<ICommand<User>>();
            LoggingCommand<User> logCmd = (LoggingCommand<User>)cmd;

            Assert.IsNotNull(logCmd.Inner);
            AssertExtensions.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<User>));
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
            AssertExtensions.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<User>));
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
            container.RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
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
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>), new InjectionConstructor())
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            ICommand<User> result = container.Resolve<ICommand<User>>();
            AssertExtensions.IsInstanceOfType(result, typeof(LoggingCommand<User>));

            ICommand<Account> accountResult = container.Resolve<ICommand<Account>>();

            AssertExtensions.IsInstanceOfType(accountResult, typeof(LoggingCommand<Account>));
        }

        [TestMethod]
        public void CanConfigureInjectionForGenericProperty()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(),
                    new InjectionProperty("Inner",
                        new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");
        }

        [TestMethod]
        public void GenericPropertyIsActuallyInjected()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                    new InjectionConstructor(),
                    new InjectionProperty("Inner",
                        new ResolvedParameter(typeof(ICommand<>), "inner")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "inner");

            ICommand<Account> result = container.Resolve<ICommand<Account>>();

            LoggingCommand<Account> actualResult = (LoggingCommand<Account>)result;

            Assert.IsNotNull(actualResult.Inner);
            AssertExtensions.IsInstanceOfType(actualResult.Inner, typeof(ConcreteCommand<Account>));
        }

        [TestMethod]
        public void CanInjectNonGenericPropertyOnGenericClass()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>),
                    new InjectionProperty("NonGenericProperty"));

            ConcreteCommand<User> result = (ConcreteCommand<User>)(container.Resolve<ICommand<User>>());
            Assert.IsNotNull(result.NonGenericProperty);
        }

        [TestMethod]
        public void CanInjectNestedGenerics()
        {
            IUnityContainer container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(LoggingCommand<>),
                new InjectionConstructor(new ResolvedParameter(typeof(ICommand<>), "concrete")))
                .RegisterType(typeof(ICommand<>), typeof(ConcreteCommand<>), "concrete");

            var cmd = container.Resolve<ICommand<Customer?>>();
            var logCmd = (LoggingCommand<Customer?>)cmd;

            Assert.IsNotNull(logCmd.Inner);
            AssertExtensions.IsInstanceOfType(logCmd.Inner, typeof(ConcreteCommand<Customer?>));
        }

        [TestMethod]
        public void ContainerControlledOpenGenericsAreDisposed()
        {
            var container = new UnityContainer()
                .RegisterType(typeof(ICommand<>), typeof(DisposableCommand<>),
                              new ContainerControlledLifetimeManager());

            var accountCommand = container.Resolve<ICommand<Account>>();
            var userCommand = container.Resolve<ICommand<User>>();

            container.Dispose();

            Assert.IsTrue(((DisposableCommand<Account>)accountCommand).Disposed);
            Assert.IsTrue(((DisposableCommand<User>)userCommand).Disposed);
        }
    }

    // Our generic interface 
    public interface ICommand<T>
    {
        void Execute(T data);
        void ChainedExecute(ICommand<T> inner);
    }

    // An implementation of ICommand that executes them.
    public class ConcreteCommand<T> : ICommand<T>
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
    public class LoggingCommand<T> : ICommand<T>
    {
        private ICommand<T> inner;

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

    // Test class for lifetime and dispose with open generics
    public class DisposableCommand<T> : ICommand<T>, IDisposable
    {
        public bool Disposed { get; private set; }

        public void Execute(T data)
        {
        }

        public void ChainedExecute(ICommand<T> inner)
        {
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }

    // A type with some nasty generics in the constructor
    public class Pathological<T1, T2>
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
    public class User
    {
        public void DoSomething(string message)
        {
        }
    }

    public class Account
    {
    }

    // Value type used for testing nesting
    public struct Customer
    {
    }
}
