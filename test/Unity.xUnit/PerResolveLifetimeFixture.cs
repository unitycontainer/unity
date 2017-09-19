// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Xunit;
#endif

namespace Unity.Tests
{
    /// <summary>
    /// Summary description for PerResolveLifetimeFixture
    /// </summary>
     
    public class PerResolveLifetimeFixture
    {
        [Fact]
        public void ContainerCanBeConfiguredForPerBuildSingleton()
        {
            var container = new UnityContainer()
                .RegisterType<IPresenter, MockPresenter>()
                .RegisterType<IView, View>(new PerResolveLifetimeManager());
        }

        [Fact]
        public void ViewIsReusedAcrossGraph()
        {
            var container = new UnityContainer()
                .RegisterType<IPresenter, MockPresenter>()
                .RegisterType<IView, View>(new PerResolveLifetimeManager());

            var view = container.Resolve<IView>();

            var realPresenter = (MockPresenter)view.Presenter;
            Assert.Same(view, realPresenter.View);
        }

        //[Fact]
        //public void ViewsAreDifferentInDifferentResolveCalls()
        //{
        //    var container = new UnityContainer()
        //        .RegisterType<IPresenter, MockPresenter>()
        //        .RegisterType<IView, View>(new PerResolveLifetimeManager());

        //    var view1 = container.Resolve<IView>();
        //    var view2 = container.Resolve<IView>();

        //    Assert.NotSame(view1, view2);
        //}

        [Fact]
        public void PerBuildLifetimeIsHonoredWhenUsingFactory()
        {
            var container = new UnityContainer()
                .RegisterType<SomeService>(
                    new PerResolveLifetimeManager(),
                    new InjectionFactory(c => new SomeService()));

            var rootService = container.Resolve<AService>();
            Assert.Same(rootService.SomeService, rootService.OtherService.SomeService);
        }

        // A small object graph to verify per-build configuration works

        public interface IPresenter { }
        public class MockPresenter : IPresenter
        {
            public IView View { get; set; }

            public MockPresenter(IView view)
            {
                View = view;
            }
        }

        public interface IView
        {
            IPresenter Presenter { get; set; }
        }

        public class View : IView
        {
            [Dependency]
            public IPresenter Presenter { get; set; }
        }

        public class SomeService { }

        public class SomeOtherService
        {
            public SomeService SomeService { get; set; }
            public SomeOtherService(SomeService someService)
            {
                this.SomeService = someService;
            }
        }

        public class AService
        {
            public AService(SomeOtherService otherService)
            {
                this.OtherService = otherService;
            }

            [Dependency]
            public SomeService SomeService { get; set; }

            public SomeOtherService OtherService { get; set; }
        }
    }
}
