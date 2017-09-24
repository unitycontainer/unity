using System.Collections.Generic;

namespace Unity.Tests.CollectionSupport
{
    public class TestClassWithEnumerableDependency
    {
        [Dependency]
        public IEnumerable<TestClass> Dependency { get; set; }
    }
}
