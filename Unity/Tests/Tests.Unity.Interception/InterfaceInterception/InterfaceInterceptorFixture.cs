using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules;
using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.InterfaceInterception
{
    [TestClass]
    public class InterfaceInterceptorFixture
    {
        [TestMethod]
        public void InterceptorDoesNotInterceptClass()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            Assert.IsFalse(interceptor.CanIntercept(GetType()));
        }

        [TestMethod]
        public void InterceptorCanInterceptInterface()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            Assert.IsTrue(interceptor.CanIntercept(typeof(IMatchingRule)));
        }

        [TestMethod]
        public void InterceptorReturnsCorrectMethodsForInterceptableType()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            List<MethodImplementationInfo> methods = Seq.Make(interceptor.GetInterceptableMethods(typeof (IInterfaceOne), typeof (ImplementationOne))).ToList();

            List<MethodImplementationInfo> expected = Seq.Collect(
                new MethodImplementationInfo(typeof (IInterfaceOne).GetMethod("TargetMethod"),
                    typeof (ImplementationOne).GetMethod("TargetMethod"))).ToList();

            CollectionAssert.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorMapsGenericMethodsOnClosedGenericInterfaces()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            List<MethodImplementationInfo> methods = Seq.Make(interceptor.GetInterceptableMethods(typeof (IGenericOne<string>), typeof (GenericImplementationOne<string>))).ToList();

            List<MethodImplementationInfo> expected = Sequence.ToList(
                GetExpectedMethods<IGenericOne<string>, GenericImplementationOne<string>>("DoSomething"));

            CollectionAssert.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void InterceptorCreatesProxyInstance()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ImplementsInterfaceOne target = new ImplementsInterfaceOne();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof (IInterfaceOne), target);

            IInterfaceOne intercepted = (IInterfaceOne) proxy;
            intercepted.TargetMethod();

            Assert.IsTrue(target.TargetMethodCalled);
        }

        [TestMethod]
        public void GeneratedProxyCallsHandlers()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            ImplementsInterfaceOne target = new ImplementsInterfaceOne();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof(IInterfaceOne), target);
            CallCountHandler handler = new CallCountHandler();
            proxy.SetPipeline(typeof (IInterfaceOne).GetMethod("TargetMethod"),
                new HandlerPipeline(new ICallHandler[] {handler}));

            IInterfaceOne intercepted = (IInterfaceOne) proxy;

            intercepted.TargetMethod();
            intercepted.TargetMethod();
            intercepted.TargetMethod();

            Assert.AreEqual(3, handler.CallCount);
            
        }

        [TestMethod]
        public void ParametersPassProperlyToTarget()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();
            MyDal target = new MyDal();

            IInterceptingProxy proxy = interceptor.CreateProxy(typeof (IDal), target);
            CallCountHandler depositHandler = new CallCountHandler();
            CallCountHandler withdrawHandler = new CallCountHandler();
            proxy.SetPipeline(typeof (IDal).GetMethod("Deposit"),
                new HandlerPipeline(new ICallHandler[] {depositHandler}));
            proxy.SetPipeline(typeof(IDal).GetMethod("Withdraw"),
                new HandlerPipeline(new ICallHandler[] { withdrawHandler }));

            IDal intercepted = (IDal) proxy;

            intercepted.Deposit(100.0);
            intercepted.Deposit(25.95);
            intercepted.Deposit(19.95);

            intercepted.Withdraw(15.00);
            intercepted.Withdraw(6.25);

            Assert.AreEqual(3, depositHandler.CallCount);
            Assert.AreEqual(2, withdrawHandler.CallCount);

            Assert.AreEqual(100.0 + 25.95 + 19.95 - 15.00 - 6.25, target.Balance);
        }


        [TestMethod]
        [Ignore] // Skipping open generic case for the moment.
        public void InterceptorMapsGenericMethodsOnOpenGenericInterfaces()
        {
            IInstanceInterceptor interceptor = new InterfaceInterceptor();

            List<MethodImplementationInfo> methods = Seq.Make(interceptor.GetInterceptableMethods(typeof (IGenericOne<>), typeof (GenericImplementationOne<>))).ToList();

            List<MethodImplementationInfo> expected = Sequence.ToList(
                GetExpectedMethods(typeof(IGenericOne<>), typeof(GenericImplementationOne<>), "DoSomething"));

            CollectionAssert.AreEquivalent(expected, methods);
        }


        private static IEnumerable<MethodImplementationInfo> GetExpectedMethods<TInterface, TImplementation>(params string[] methodNames)
        {
            return GetExpectedMethods(typeof (TInterface), typeof (TImplementation), methodNames);
        }

        private static IEnumerable<MethodImplementationInfo> GetExpectedMethods(Type interfaceType, Type implementationType, params string[] methodNames)
        {
            foreach (string name in methodNames)
            {
                yield return new MethodImplementationInfo(
                    interfaceType.GetMethod(name),
                    implementationType.GetMethod(name));

            }
        }
        internal class ImplementationOne : IInterfaceOne
        {
            public void TargetMethod()
            {
                
            }
        }

        // Experimenting with reflection and open generics. Oy, what a pain.
        [TestMethod]
        public void OpenGenericImplementsOpenGenericInterface()
        {
            Type implementationType = typeof (GenericImplementationOne<>);
            Type interfaceType = typeof (IGenericOne<>);

            Assert.AreSame(interfaceType, implementationType.GetInterfaces()[0].GetGenericTypeDefinition());

            Type genericArgument = implementationType.GetGenericArguments()[0];
            Type implementedInterfaceType = interfaceType.MakeGenericType(genericArgument);

            InterfaceMapping mapping = implementationType.GetInterfaceMap(implementedInterfaceType);
            Assert.AreEqual(1, mapping.InterfaceMethods.Length );
            Assert.AreSame(implementedInterfaceType.GetMethod("DoSomething"), mapping.InterfaceMethods[0]);
            Assert.AreSame(typeof (GenericImplementationOne<>).GetMethod("DoSomething"), mapping.TargetMethods[0]);

        }


        internal interface IGenericOne<T>
        {
            T DoSomething(T something);
        }

        internal class GenericImplementationOne<T> : IGenericOne<T>
        {
            public T DoSomething(T something)
            {
                throw new System.NotImplementedException();
            }
        }

        class ImplementsInterfaceOne : IInterfaceOne
        {
            public bool TargetMethodCalled = false;

            public void TargetMethod()
            {
                TargetMethodCalled = true;    
            }
        }

        class MyDal : IDal
        {
            private double balance;

            public double Balance { get { return balance; } }

            public void Deposit(double amount)
            {
                balance += amount;
            }

            public void Withdraw(double amount)
            {
                balance -= amount;
            }
        }

        /// <summary>
        /// Test class used to reverse engineer required generated code.
        /// </summary>
        class IntefaceOneInterceptor : IInterceptingProxy, IInterfaceOne
        {
            PipelineManager pipelines;
            private IInterfaceOne target;

            public IntefaceOneInterceptor(IInterfaceOne target)
            {
                this.target = target;
                pipelines = new PipelineManager();
            }

            HandlerPipeline IInterceptingProxy.GetPipeline(MethodBase method)
            {
                return pipelines.GetPipeline(method.MetadataToken);
            }

            void IInterceptingProxy.SetPipeline(MethodBase method, HandlerPipeline pipeline)
            {
                pipelines.SetPipeline(method.MetadataToken, pipeline);
            }

            public void TargetMethod()
            {
                MethodInfo targetMethod = typeof (IInterfaceOne).GetMethod("TargetMethod");

                VirtualMethodInvocation input = new VirtualMethodInvocation(target, targetMethod);
                HandlerPipeline pipeline = ((IInterceptingProxy)this).GetPipeline(targetMethod);
                IMethodReturn returnMessage = pipeline.Invoke(input, delegate(IMethodInvocation inputs, GetNextHandlerDelegate getNext)
                {
                    try
                    {
                        target.TargetMethod();
                        return inputs.CreateMethodReturn(null);
                    }
                    catch (Exception ex)
                    {

                        return inputs.CreateExceptionMethodReturn(ex);
                    }
                });

                if(returnMessage.Exception != null)
                {
                    throw returnMessage.Exception;
                }
            }
        }
    }
}