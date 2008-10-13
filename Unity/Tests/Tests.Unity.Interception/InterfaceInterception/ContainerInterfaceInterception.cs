using Microsoft.Practices.Unity.InterceptionExtension.Tests.ObjectsUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSupport.Unity;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.InterfaceInterception
{
    [TestClass]
    public class ContainerInterfaceInterception
    {
        [TestMethod]
        public void CanInterceptInstancesViaTheContainer()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<Interception>()
                .RegisterType<IDal, MockDal>()
                .Configure<Interception>()
                    .SetInterceptorFor<IDal>(new InterfaceInterceptor())
                    .AddPolicy("AlwaysMatches")
                    .AddMatchingRule<AlwaysMatchingRule>()
                    .AddCallHandler<CallCountHandler>("callCount", new ContainerControlledLifetimeManager())
                    .Interception
                .Container;

            IDal dal = container.Resolve<IDal>();

            Assert.IsTrue(dal is IInterceptingProxy);

            dal.Deposit(50.0);
            dal.Deposit(65.0);
            dal.Withdraw(15.0);

            CallCountHandler handler = (CallCountHandler)(container.Resolve<ICallHandler>("callCount"));
            Assert.AreEqual(3, handler.CallCount);

        }

    }
}
