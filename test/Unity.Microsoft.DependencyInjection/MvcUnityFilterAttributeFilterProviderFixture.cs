//// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

//using System;
//using System.Linq;
//using System.Web.Mvc;
//using Unity.Mvc;
//using Microsoft.Practices.Unity;
//using Xunit;

//namespace Microsoft.Practices.Unity.WebIntegation.Tests
//{
//
//    public class MvcUnityFilterAttributeFilterProviderFixture
//    {
//        [Fact]
//        public void When_getting_action_attributes_then_builds_up_instance()
//        {
//            using (var container = new UnityContainer())
//            {
//                // Arrange
//                var someInstance = new SomeClass();
//                container.RegisterInstance<ISomeInterface>(someInstance);
//                container.RegisterType<TestFilterAttribute>(new InjectionProperty("Some"));

//                var context = new ControllerContext { Controller = new ControllerWithActionAttribute() };
//                var controllerDescriptor = new ReflectedControllerDescriptor(context.Controller.GetType());
//                var action = context.Controller.GetType().GetMethod("MyActionMethod");
//                var actionDescriptor = new ReflectedActionDescriptor(action, "MyActionMethod", controllerDescriptor);
//                var provider = new UnityFilterAttributeFilterProvider(container);

//                // Act
//                Filter filter = provider.GetFilters(context, actionDescriptor).Single();

//                // Assert
//                TestFilterAttribute attrib = filter.Instance as TestFilterAttribute;
//                Assert.IsNotNull(attrib);
//                Assert.AreEqual(FilterScope.Action, filter.Scope);
//                Assert.AreEqual(1234, filter.Order);
//                Assert.AreSame(someInstance, attrib.Some);
//            }
//        }

//        [Fact]
//        public void When_getting_controller_attributes_then_builds_up_instance()
//        {
//            using (var container = new UnityContainer())
//            {
//                // Arrange
//                var someInstance = new SomeClass();
//                container.RegisterInstance<ISomeInterface>(someInstance);
//                container.RegisterType<TestFilterAttribute>(new InjectionProperty("Some"));

//                var context = new ControllerContext { Controller = new ControllerWithTypeAttribute() };
//                var controllerDescriptor = new ReflectedControllerDescriptor(context.Controller.GetType());
//                var action = context.Controller.GetType().GetMethod("MyActionMethod");
//                var actionDescriptor = new ReflectedActionDescriptor(action, "MyActionMethod", controllerDescriptor);
//                var provider = new UnityFilterAttributeFilterProvider(container);

//                // Act
//                Filter filter = provider.GetFilters(context, actionDescriptor).Single();

//                // Assert
//                TestFilterAttribute attrib = filter.Instance as TestFilterAttribute;
//                Assert.NotNull(attrib);
//                Assert.Equal(FilterScope.Controller, filter.Scope);
//                Assert.Equal(1234, filter.Order);
//                Assert.Same(someInstance, attrib.Some);
//            }
//        }

//        private class TestFilterAttribute : FilterAttribute
//        {
//            public ISomeInterface Some { get; set; }
//        }

//        [TestFilter(Order = 1234)]
//        private class ControllerWithTypeAttribute : Controller
//        {
//            public ActionResult MyActionMethod()
//            {
//                return null;
//            }
//        }

//        private class ControllerWithActionAttribute : Controller
//        {
//            [TestFilter(Order = 1234)]
//            public ActionResult MyActionMethod()
//            {
//                return null;
//            }
//        }

//        public interface ISomeInterface
//        {
//        }

//        public class SomeClass : ISomeInterface
//        {
//        }
//    }
//}
