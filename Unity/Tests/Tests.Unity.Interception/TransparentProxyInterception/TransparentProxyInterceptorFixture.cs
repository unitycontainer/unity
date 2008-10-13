using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.TransparentProxyInterception
{
    [TestClass]
    public class TransparentProxyInterceptorFixture
    {
        [TestMethod]
        public void InterceptorReturnsSingleMethod()
        {
            List<MethodImplementationInfo> methods = GetMethods<SingleInterceptableMethod>();

            CollectionAssert.AreEquivalent(GetOnlyImplementations(typeof (SingleInterceptableMethod), "MyMethod"),
                methods);
        }

        [TestMethod]
        public void InterceptorReturnsAllMethodsIncludingInheritedOnes()
        {
            List<MethodImplementationInfo> methods = GetMethods<InheritsSingleMethodAndAdds>();

            CollectionAssert.AreEquivalent(
                GetOnlyImplementations(typeof (InheritsSingleMethodAndAdds), "MyMethod", "AnotherMethod", "StillMoreMethod"),
                methods);
        }

        [TestMethod]
        public void NonMBROReturnsInterfaceMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Operations>();

            Assert.AreEqual(2, methods.Count);

            List<MethodImplementationInfo> expected = GetExpectedMethodImplementations(typeof (IMyOperations), typeof (Operations));

            CollectionAssert.AreEquivalent(expected, methods);
        }

        [TestMethod]
        public void ReturnsAllInterfaceMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Incoherent>();

            CollectionAssert.AreEquivalent(
                Seq.Make(GetExpectedMethodImplementations(typeof (IMyOperations), typeof (Incoherent)))
                    .Concat(GetExpectedMethodImplementations(typeof (ImTiredOfInterfaces), typeof (Incoherent))).ToList(),
                methods);
        }

        [TestMethod]
        public void EmptyInterfacesContributeNoMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<Marked>();

            CollectionAssert.AreEquivalent(
                GetExpectedMethodImplementations(typeof (IMyOperations), typeof (Marked)),
                methods);
        }

        [TestMethod]
        public void PropertiesAreReturnedAsGetAndSetMethods()
        {
            List<MethodImplementationInfo> methods = GetMethods<HasProperties>();

            CollectionAssert.AreEquivalent(
                GetOnlyImplementations(typeof (HasProperties), "get_SettableProp", "set_SettableProp", "get_GetOnly"),
                methods);
        }

        [TestMethod]
        public void InterfacePropertiesAreReturned()
        {
            List<MethodImplementationInfo> methods = GetMethods<PropThroughInterface>();

            CollectionAssert.AreEquivalent(
                GetExpectedMethodImplementations(typeof (IHasProperties), typeof (PropThroughInterface)),
                methods);
        }

        [TestMethod]
        public void MBROReturnBothInterfaceAndClassProperties()
        {
            List<MethodImplementationInfo> methods = GetMethods<MBROWithPropThroughInterface>();
            
            Seq<MethodImplementationInfo> expected = Seq.Make(
                GetExpectedMethodImplementations(typeof (IHasProperties), typeof (MBROWithPropThroughInterface)))
                .Concat(GetOnlyImplementations(typeof (MBROWithPropThroughInterface), "get_Gettable"));
            CollectionAssert.AreEquivalent(expected.ToList(), methods);
        }

        [TestMethod]
        public void ExplicitImplementationsAreFound()
        {
            List<MethodImplementationInfo> methods = GetMethods<ExplicitImplementation>();

            Seq<MethodImplementationInfo> expected = 
                Seq.Make(GetExpectedMethodImplementations(typeof (IMyOperations), typeof (ExplicitImplementation)))
                .Concat(GetOnlyImplementations(typeof(ExplicitImplementation), "AClassMethod"));
            CollectionAssert.AreEquivalent(expected.ToList(), methods);
        }

        private static List<MethodImplementationInfo> GetMethods<T>()
        {
            TransparentProxyInterceptor interceptor = new TransparentProxyInterceptor();
            return new List<MethodImplementationInfo>(interceptor.GetInterceptableMethods(typeof(T), typeof(T)));
        }

        static List<MethodImplementationInfo> GetExpectedMethodImplementations(Type interfaceType, Type implementationType)
        {
            InterfaceMapping mapping = implementationType.GetInterfaceMap(interfaceType);
            List<MethodImplementationInfo> results = new List<MethodImplementationInfo>(mapping.InterfaceMethods.Length);

            for(int i = 0; i < mapping.InterfaceMethods.Length; ++i)
            {
                results.Add(new MethodImplementationInfo(mapping.InterfaceMethods[i], mapping.TargetMethods[i]));
            }
            return results;
        }

        static List<MethodImplementationInfo> GetOnlyImplementations(Type implementationType, params string[] methodNames)
        {
            return
                Seq.Make(methodNames).Map<MethodImplementationInfo>(delegate(string methodName)
                {
                    return new MethodImplementationInfo(null, implementationType.GetMethod(methodName));
                }).ToList();
        }
    }

    class SingleInterceptableMethod : MarshalByRefObject
    {
        public void MyMethod() { }
    }

    class InheritsSingleMethodAndAdds : SingleInterceptableMethod
    {
        public void AnotherMethod()
        {
        }

        public void StillMoreMethod()
        {
            
        }
    }

    interface IMyOperations
    {
        void Add();
        void Multiply();
    }

    interface ImTiredOfInterfaces
    {
        void YaddaYadda();    
    }

    interface IMarkerInterface
    {
        
    }

    class Operations : IMyOperations
    {
        public void Add()
        {
        }

        public void Multiply()
        {
        }

        public void NotAnInterfaceMethod()
        {
            
        }
    }

    class Incoherent : IMyOperations, ImTiredOfInterfaces
    {
        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void Multiply()
        {
            throw new System.NotImplementedException();
        }

        public void YaddaYadda()
        {
            throw new System.NotImplementedException();
        }
    }

    class Marked : IMyOperations, IMarkerInterface
    {
        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void Multiply()
        {
            throw new System.NotImplementedException();
        }
    }

    class HasProperties : MarshalByRefObject
    {
        public string SettableProp
        {
            get { return null; }
            set { }
        }

        public int GetOnly
        {
            get { return 42; }
        }
    }

    interface IHasProperties
    {
        string StringProp { get; set; }
    }

    class PropThroughInterface : IHasProperties
    {
        public string StringProp
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }

    class MBROWithPropThroughInterface : MarshalByRefObject, IHasProperties
    {
        public string StringProp
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public int Gettable
        {
            get { return 37;  }
        }
    }

    class ExplicitImplementation : MarshalByRefObject, IMyOperations
    {
        void IMyOperations.Add()
        {
            throw new System.NotImplementedException();
        }

        void IMyOperations.Multiply()
        {
            throw new System.NotImplementedException();
        }

        public void AClassMethod()
        {
            
        }
    }
}
