// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Tests.TestObjects;
using Unity.TestSupport;
using Xunit;

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for BuildPlanAndChildContainerFixture
    /// </summary>
     
    public class BuildPlanAndChildContainerFixture
    {
        private IUnityContainer parentContainer;
        private IUnityContainer childContainer;

        private const int ValueInjectedFromParent = 3;
        private const int ValueInjectedFromChild = 5;

        public BuildPlanAndChildContainerFixture()
        {
            parentContainer = new UnityContainer()
                .RegisterType<TestObject>(new InjectionConstructor(ValueInjectedFromParent))
                .RegisterType<ILogger, MockLogger>();

            childContainer = parentContainer.CreateChildContainer()
                .RegisterType<TestObject>(new InjectionConstructor(ValueInjectedFromChild));
        }

        public class TestObject
        {
            public int Value { get; private set; }

            public TestObject(int value)
            {
                Value = value;
            }
        }

        [Fact]
        public void ValuesInjectedAreCorrectWhenResolvingFromParentFirst()
        {
            // Be aware ordering is important here - resolve through parent first
            // to elicit the buggy behavior
            var fromParent = parentContainer.Resolve<TestObject>();
            var fromChild = childContainer.Resolve<TestObject>();

            Assert.Equal(ValueInjectedFromParent, fromParent.Value);
            Assert.Equal(ValueInjectedFromChild, fromChild.Value);
        }

        [Fact]
        public void ChildContainersForUnconfiguredTypesPutConstructorParamResolversInParent()
        {
            childContainer.Resolve<ObjectWithOneDependency>();
            parentContainer.CreateChildContainer().Resolve<ObjectWithOneDependency>();

            // No exception means we're good.
        }

        [Fact]
        public void ChildContainersForUnconfiguredTypesPutPropertyResolversInParent()
        {
            childContainer.Resolve<ObjectWithLotsOfDependencies>();
            parentContainer.CreateChildContainer().Resolve<ObjectWithLotsOfDependencies>();
        }
    }
}
