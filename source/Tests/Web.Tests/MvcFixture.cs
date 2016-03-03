using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Unity;
using Unity.Mvc;

namespace Web.Tests
{
    public class MvcFixture
    {
        UnityContainer defaultContainer;
        IServiceProvider defaultServiceProvider;

        public MvcFixture()
        {
            defaultContainer = new UnityContainer();
            defaultServiceProvider = new ServiceProvider(defaultContainer);

            // Register SampleClassA, but not SampleClassB
            defaultContainer.RegisterType<ISample, SampleClassA>();
        }

        [Fact]
        public void IfAServiceIsNotRegisteredTryUsingAlternativeContainer()
        {
            UnityContainer container = new UnityContainer();
            container.AlternativeServiceProvider = defaultServiceProvider;

            Assert.NotNull(defaultContainer.Resolve<ISample>());
            Assert.IsType<SampleClassA>(defaultContainer.Resolve<ISample>());
            Assert.IsNotType<SampleClassB>(defaultContainer.Resolve<ISample>());

            // Now with the other container
            Assert.NotNull(container.Resolve<ISample>());
            Assert.IsType<SampleClassA>(container.Resolve<ISample>());
            Assert.IsNotType<SampleClassB>(container.Resolve<ISample>());
        }
    }

    public interface ISample
    {
        string SampleMethod();
    }

    public interface ISample2
    {
        string SampleMethod();
    }

    public class SampleClassA : ISample
    {
        public string SampleMethod()
        {
            return "A";
        }
    }

    public class SampleClassB : ISample
    {
        public string SampleMethod()
        {
            return "B";
        }
    }
}
