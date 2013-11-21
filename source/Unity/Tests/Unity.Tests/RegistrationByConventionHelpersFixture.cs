// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.Unity.TestSupport;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class RegistrationByConventionHelpersFixture
    {
        [TestMethod]
        public void GetsNoTypes()
        {
            WithMappings.None(typeof(TypeWithoutInterfaces)).AssertHasNoItems();
            WithMappings.None(typeof(DisposableType)).AssertHasNoItems();
            WithMappings.None(typeof(TestObject)).AssertHasNoItems();
            WithMappings.None(typeof(AnotherTestObject)).AssertHasNoItems();
        }

        [TestMethod]
        public void GetsAllInterfaces()
        {
            WithMappings.FromAllInterfaces(typeof(TypeWithoutInterfaces)).AssertHasNoItems();
            WithMappings.FromAllInterfaces(typeof(DisposableType)).AssertHasNoItems();
            WithMappings.FromAllInterfaces(typeof(TestObject)).AssertContainsInAnyOrder(typeof(IAnotherInterface), typeof(ITestObject), typeof(IComparable));
            WithMappings.FromAllInterfaces(typeof(AnotherTestObject)).AssertContainsInAnyOrder(typeof(IAnotherInterface), typeof(ITestObject), typeof(IComparable));

            // Generics
            WithMappings.FromAllInterfaces(typeof(GenericTestObject<,>)).AssertContainsInAnyOrder(typeof(IGenericTestObject<,>));
            WithMappings.FromAllInterfaces(typeof(GenericTestObjectAlt<,>)).AssertHasNoItems();
            WithMappings.FromAllInterfaces(typeof(GenericTestObject<>)).AssertContainsInAnyOrder(typeof(IComparable<>));
            WithMappings.FromAllInterfaces(typeof(GenericTestObject)).AssertContainsInAnyOrder(typeof(IGenericTestObject<string, int>), typeof(IComparable<int>), typeof(IEnumerable<IList<string>>), typeof(IEnumerable));
        }

        [TestMethod]
        public void GetsMatchingInterface()
        {
            WithMappings.FromMatchingInterface(typeof(TypeWithoutInterfaces)).AssertHasNoItems();
            WithMappings.FromMatchingInterface(typeof(DisposableType)).AssertHasNoItems();
            WithMappings.FromMatchingInterface(typeof(TestObject)).AssertContainsInAnyOrder(typeof(ITestObject));
            WithMappings.FromMatchingInterface(typeof(AnotherTestObject)).AssertHasNoItems();

            // Generics
            WithMappings.FromMatchingInterface(typeof(GenericTestObject<,>)).AssertContainsExactly(typeof(IGenericTestObject<,>));
            WithMappings.FromMatchingInterface(typeof(GenericTestObjectAlt<,>)).AssertHasNoItems();
            WithMappings.FromMatchingInterface(typeof(GenericTestObject<>)).AssertHasNoItems();
            WithMappings.FromMatchingInterface(typeof(GenericTestObject)).AssertHasNoItems();
        }

        [TestMethod]
        public void GetsAllInterfacesInSameAssembly()
        {
            WithMappings.FromAllInterfacesInSameAssembly(typeof(TypeWithoutInterfaces)).AssertHasNoItems();
            WithMappings.FromAllInterfacesInSameAssembly(typeof(DisposableType)).AssertHasNoItems();
            WithMappings.FromAllInterfacesInSameAssembly(typeof(TestObject)).AssertContainsInAnyOrder(typeof(ITestObject), typeof(IAnotherInterface));
            WithMappings.FromAllInterfacesInSameAssembly(typeof(AnotherTestObject)).AssertContainsInAnyOrder(typeof(ITestObject), typeof(IAnotherInterface));

            // Generics
            WithMappings.FromAllInterfacesInSameAssembly(typeof(GenericTestObject<,>)).AssertContainsExactly(typeof(IGenericTestObject<,>));
            WithMappings.FromAllInterfacesInSameAssembly(typeof(GenericTestObjectAlt<,>)).AssertHasNoItems();
            WithMappings.FromAllInterfacesInSameAssembly(typeof(GenericTestObject<>)).AssertHasNoItems();
            WithMappings.FromAllInterfacesInSameAssembly(typeof(GenericTestObject)).AssertContainsExactly(typeof(IGenericTestObject<string, int>));
        }

        [TestMethod]
        public void GetsNames()
        {
            Assert.AreEqual("MockLogger", WithName.TypeName(typeof(MockLogger)));
            Assert.AreEqual("List`1", WithName.TypeName(typeof(List<>)));
            Assert.IsNull(WithName.Default(typeof(MockLogger)));
            Assert.IsNull(WithName.Default(typeof(List<>)));
        }

        [TestMethod]
        public void GetsLifetimeManagers()
        {
            Assert.IsInstanceOfType(WithLifetime.ContainerControlled(typeof(MockLogger)), typeof(ContainerControlledLifetimeManager));
            Assert.IsInstanceOfType(WithLifetime.ExternallyControlled(typeof(MockLogger)), typeof(ExternallyControlledLifetimeManager));
            Assert.IsInstanceOfType(WithLifetime.Hierarchical(typeof(MockLogger)), typeof(HierarchicalLifetimeManager));
            Assert.IsNull(WithLifetime.None(typeof(MockLogger)));
            Assert.IsInstanceOfType(WithLifetime.PerResolve(typeof(MockLogger)), typeof(PerResolveLifetimeManager));
            Assert.IsInstanceOfType(WithLifetime.Transient(typeof(MockLogger)), typeof(TransientLifetimeManager));
            Assert.IsInstanceOfType(WithLifetime.Custom<CustomLifetimeManager>(typeof(MockLogger)), typeof(CustomLifetimeManager));

#if !NETFX_CORE
            Assert.IsInstanceOfType(WithLifetime.PerThread(typeof(MockLogger)), typeof(PerThreadLifetimeManager));
#endif
        }

        public class CustomLifetimeManager : LifetimeManager
        {
            public override object GetValue()
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object newValue)
            {
                throw new NotImplementedException();
            }

            public override void RemoveValue()
            {
                throw new NotImplementedException();
            }
        }


        public class TypeWithoutInterfaces { }

        public class DisposableType : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        public class TestObject : IAnotherInterface, ITestObject, IDisposable, IComparable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        public class AnotherTestObject : IAnotherInterface, ITestObject, IDisposable, IComparable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }
        }

        public interface ITestObject { }

        public interface IAnotherInterface { }

        public interface IGenericTestObject<T, U> { }

        public class GenericTestObject<T, U> : IGenericTestObject<T, U>, IComparable<T>, IEnumerable<IList<T>>
        {
            public int CompareTo(T other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<IList<T>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class GenericTestObjectAlt<T, U> : IGenericTestObject<U, T>
        {
        }

        public class GenericTestObject<T> : IGenericTestObject<T, int>, IComparable, IEnumerable<IList<int>>, IComparable<T>
        {
            public int CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

            public int CompareTo(T other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<IList<int>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class GenericTestObject : IGenericTestObject<string, int>, IComparable<int>, IEnumerable<IList<string>>
        {
            public int CompareTo(int other)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<IList<string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
