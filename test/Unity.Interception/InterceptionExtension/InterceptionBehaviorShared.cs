// TODO: Verify
//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//using Microsoft.Practices.Unity.InterceptionExtension;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Unity.Tests.InterceptionExtension
//{
//    public interface IMock
//    {
//        [MyCallHandler]
//        string DoSomething(string param1);
//    }

//    public class MockClass : IMock
//    {
//        public string DoSomething(string param1)
//        {
//            return param1;
//        }
//    }

//    public interface IMockAnother
//    {
//        int DoSomethingWithInt(int a);
//    }

//    public class MyCallHandler : ICallHandler
//    {
//        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
//        {
//            var p = input.Arguments[0].ToString();
//            p += " I've been here!";

//            input.Arguments[0] = p;
//            return getNext()(input, getNext);
//        }

//        public int Order { get; set; }
//    }

//    public class MyCallHandlerAttribute : HandlerAttribute
//    {
//        public override ICallHandler CreateHandler(Microsoft.Practices.Unity.IUnityContainer container)
//        {
//            return new MyCallHandler();
//        }
//    }

//    public class ClassWithNonPublicVirtualProperties
//    {
//        protected virtual string SomeProperty
//        {
//            get
//            {
//                return "intercepted";
//            }
//        }

//        public virtual string GetSomePropertyValue()
//        {
//            return SomeProperty;
//        }
//    }

//    public abstract class MyBaseClass
//    {
//        public virtual string DoSomething()
//        {
//            return "from base";
//        }
//    }

//    public class MyDerivedClass : MyBaseClass
//    {
//        public override string DoSomething()
//        {
//            return "derived";
//        }
//    }

//    public abstract class BadRequiredInterface
//    {
//    }

//    public interface IGenericInterface<T>
//    {
//    }

//    public class InterceptionBehaviorWithGenericClass<T> : IInterceptionBehavior
//    {
//        private Type[] requiredInterfaces = new Type[1];

//        public InterceptionBehaviorWithGenericClass()
//        {
//            requiredInterfaces[0] = typeof(IGenericInterface<string>).GetGenericTypeDefinition();
//        }

//        public IEnumerable<Type> GetRequiredInterfaces()
//        {
//            return requiredInterfaces;
//        }

//        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
//        {
//            return getNext()(input, getNext);
//        }

//        public bool WillExecute
//        {
//            get { return true; }
//        }
//    }

//    public class BadInterceptionBehavior : IInterceptionBehavior
//    {
//        private Type[] requiredInterfaces = new[] { typeof(BadRequiredInterface) };

//        public BadInterceptionBehavior()
//        {
//        }

//        public IEnumerable<Type> GetRequiredInterfaces()
//        {
//            return requiredInterfaces;
//        }

//        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
//        {
//            return getNext()(input, getNext);
//        }

//        public bool WillExecute
//        {
//            get { return true; }
//        }
//    }

//    public class ClassWithInjectedStuff
//    {
//        private string constructorData;
//        private string propertyData;
//        public bool ConstructorInvoked = false;

//        public ClassWithInjectedStuff(string constructorData)
//        {
//            this.constructorData = constructorData;
//            this.ConstructorInvoked = true;
//        }

//        public string Data
//        {
//            get { return this.propertyData; }
//            set { this.propertyData = value; }
//        }
//    }

//    public class MyInterceptionBehavior : IInterceptionBehavior
//    {
//        private Type[] requiredInterfaces = new[] { typeof(IMockAnotherBase) };
//        private Action actionToDo = null;

//        public MyInterceptionBehavior() :
//            this(null)
//        {
//        }

//        public MyInterceptionBehavior(Action action)
//        {
//            this.actionToDo = action;
//        }

//        public IEnumerable<Type> GetRequiredInterfaces()
//        {
//            return requiredInterfaces;
//        }

//        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
//        {
//            actionToDo.Invoke();

//            return getNext()(input, getNext);
//        }

//        public bool WillExecute
//        {
//            get { return true; }
//        }
//    }

//    public interface IMockAnotherBase
//    {
//        string Data { get; }
//    }
//}
