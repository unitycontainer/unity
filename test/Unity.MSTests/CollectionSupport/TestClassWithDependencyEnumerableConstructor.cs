using System.Collections.Generic;

namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithDependencyEnumerableConstructor
    {
        public IEnumerable<TestClass> Dependency { get; set; }

        public TestClassWithDependencyEnumerableConstructor(IEnumerable<TestClass> dependency)
        {
            Dependency = dependency;
        }
    }
}
