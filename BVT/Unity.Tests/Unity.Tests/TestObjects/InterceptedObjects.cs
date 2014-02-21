// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Unity.Tests.TestObjects
{
    public interface IAdditionalInterface
    {
        int DoNothing();
    }

    public interface IAdditionalInterface1
    {
        int DoNothing1();
    }

    public class BehavourForAdditionalInterface : IInterceptionBehavior
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.Name == "DoNothing")
            {
                return input.CreateMethodReturn(100);
            }
            if (input.MethodBase.Name == "DoNothing1")
            {
                return input.CreateMethodReturn(200);
            }

            return getNext()(input, getNext);
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return new Type[] { typeof(IAdditionalInterface) };
        }

        public bool WillExecute
        {
            get { return true; }
        }
    }

    public interface IInterfaceA
    {
        void TargetMethod();
    }

    public class ImplementsInterface : IInterfaceA
    {
        public bool MethodCalled = false;

        public void TargetMethod()
        {
            MethodCalled = true;
        }
    }

    public class NewImplementsInterface : IInterfaceA
    {
        public bool MethodCalled = false;

        public void TargetMethod()
        {
            this.MethodCalled = true;
        }
    }

    public interface IInterfaceB
    {
        string TargetMethod(ref string b);
    }

    public class ImplementsInterfaceB : IInterfaceB
    {
        public bool MethodCalled = false;

        public string TargetMethod(ref string b)
        {
            MethodCalled = true;
            b = b + "target";
            return b;
        }
    }

    public class NewImplementsInterfaceB : IInterfaceB
    {
        public bool MethodCalled = false;

        public string TargetMethod(ref string b)
        {
            MethodCalled = true;
            b = b + "target";
            return b;
        }
    }

    public interface IInterfaceTest
    {
        string TargetMethod(ref string b);
        string SecondTargetMethod(ref string b);
        string ThirdTargetMethod(ref string b);
    }

    public class TestClassForMultipleInterception : MarshalByRefObject, IInterfaceTest
    {
        public bool MethodCalled = false;

        public string TargetMethod(ref string b)
        {
            this.MethodCalled = true;
            b = b + "target";
            return b;
        }

        public string SecondTargetMethod(ref string b)
        {
            this.MethodCalled = true;
            b = b + "target";
            return b;
        }

        public virtual string ThirdTargetMethod(ref string b)
        {
            this.MethodCalled = true;
            b = b + "target";
            return b;
        }
    }

    public class DoNothingInterceptionBehavior : IInterceptionBehavior
    {
        public static string PreCalled;
        public static string PostCalled;
        private Type[] requiredInterfaces = Type.EmptyTypes;
        public static string BehaviourName;

        public DoNothingInterceptionBehavior()
        {
        }

        public DoNothingInterceptionBehavior(string behaviourName)
        {
            BehaviourName = behaviourName;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            PreCalled = "Called";

            IMethodReturn returnValue = getNext()(input, getNext);
            PostCalled = "Called";
            return returnValue;
        }

        public static void Reset()
        {
            PreCalled = String.Empty;
            PostCalled = String.Empty;
        }
    }

    public class AInterceptionBehavior : IInterceptionBehavior
    {
        public static string PreCalled;
        public static string PostCalled;
        private Type[] requiredInterfaces = Type.EmptyTypes;
        public static string BehaviourName;
        public AInterceptionBehavior()
        {
            BehaviourName = "AInterceptionBehavior";
        }

        public AInterceptionBehavior(string behaviourName)
        {
            BehaviourName = behaviourName;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.Name == "TargetMethod" || input.MethodBase.Name == "SecondTargetMethod" || input.MethodBase.Name == "ThirdTargetMethod")
            {
                PreCalled = "CalledA";
                input.Inputs[0] = (string)input.Inputs[0] + "PreA";
                IMethodReturn returnValue = getNext()(input, getNext);
                input.Inputs[0] = (string)input.Inputs[0] + "PostA";

                PostCalled = "CalledA";
                return returnValue;
            }
            return getNext()(input, getNext);
        }

        public static void Reset()
        {
            PreCalled = String.Empty;
            PostCalled = String.Empty;
        }
    }

    public class BInterceptionBehavior : IInterceptionBehavior
    {
        public static string PreCalled;
        public static string PostCalled;
        private Type[] requiredInterfaces = Type.EmptyTypes;
        public static string BehaviourName;
        public BInterceptionBehavior()
        {
            BehaviourName = "BInterceptionBehavior";
        }

        public BInterceptionBehavior(string behaviourName)
        {
            BehaviourName = behaviourName;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.Name == "TargetMethod" || input.MethodBase.Name == "SecondTargetMethod" || input.MethodBase.Name == "ThirdTargetMethod")
            {
                PreCalled = "CalledB";
                input.Inputs[0] = (string)input.Inputs[0] + "PreB";
                IMethodReturn returnValue = getNext()(input, getNext);
                input.Inputs[0] = (string)input.Inputs[0] + "PostB";

                PostCalled = "CalledB";
                return returnValue;
            }
            return getNext()(input, getNext);
        }

        public static void Reset()
        {
            PreCalled = String.Empty;
            PostCalled = String.Empty;
        }
    }

    public class CInterceptionBehavior : IInterceptionBehavior
    {
        public static string PreCalled;
        public static string PostCalled;
        private Type[] requiredInterfaces = Type.EmptyTypes;
        public static string BehaviourName;
        
        public CInterceptionBehavior()
        {
            BehaviourName = "CInterceptionBehavior";
        }

        public CInterceptionBehavior(string behaviourName)
        {
            BehaviourName = behaviourName;
        }

        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return requiredInterfaces;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input.MethodBase.Name == "TargetMethod" || input.MethodBase.Name == "SecondTargetMethod" || input.MethodBase.Name == "ThirdTargetMethod")
            {
                PreCalled = "CalledC";
                input.Inputs[0] = (string)input.Inputs[0] + "PreC";
                IMethodReturn returnValue = getNext()(input, getNext);
                input.Inputs[0] = (string)input.Inputs[0] + "PostC";

                PostCalled = "CalledC";
                return returnValue;
            }
            return getNext()(input, getNext);
        }

        public static void Reset()
        {
            PreCalled = String.Empty;
            PostCalled = String.Empty;
            BehaviourName = String.Empty;
        }
    }

    public class ImplementsMBRO : MarshalByRefObject
    {
        public bool TargetMethodCalled;

        public void TargetMethod()
        {
            this.TargetMethodCalled = true;
        }
    }

    public class HasVirtualMethods
    {
        public bool TargetMethodCalled;

        public virtual void TargetMethod()
        {
            this.TargetMethodCalled = true;
        }
    }

    public class HasVirtualMethodsTest
    {
        public bool TargetMethodCalled;

        [Tag("Test")]
        public virtual void TargetMethod(string inputParam1, string inputParam2, out string outParam1, out string outParam2, ref string refParam1, ref string refParam2)
        {
            outParam1 = String.Empty;
            outParam2 = String.Empty;
            outParam1 = outParam1 + "inside target method outparam1";
            outParam2 = outParam2 + "inside target method outparam2";
            
            if (inputParam1 != "inputParam1")
            {
                throw new InvalidOperationException("inputParam1");
            }
            
            if (inputParam2 != "inputParam2")
            {
                throw new InvalidOperationException("inputParam2");
            }
            
            if (refParam1 != "refParam1")
            {
                throw new InvalidOperationException("refParam1");
            }
            
            if (refParam2 != "refParam2") 
            {
                throw new InvalidOperationException("refParam2");
            }
            refParam1 = refParam1 + "inside target method refParam1";
            refParam2 = refParam2 + "inside target method refParam2";
        }
    }

    public interface IMyInterface
    {
        event EventHandler<MyArg> E1;
        void TargetMethod();
    }

    public class MyArg : EventArgs
    {
        public bool Status = false;
    }

    public class MyClass : IMyInterface
    {
        public virtual event EventHandler<MyArg> E1;

        public virtual void TargetMethod()
        {
            MyArg arg = new MyArg();
            arg.Status = true;

            E1(null, arg);
        }
    }

    public interface ITestInterface1
    {
        int TargetMethod();
    }

    public class TestClass123 : MarshalByRefObject, ITestInterface1
    {
        public virtual int TargetMethod()
        {
            return -1;
        }
    }
}