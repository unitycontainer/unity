// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using EventBrokerExtension;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEventBroker;
using Tests.EventBrokerExtension.TestObjects;

namespace Tests.EventBrokerExtension
{
    [TestClass]
    public class SimpleEventBrokerExtensionFixture
    {
        [TestMethod]
        public void ContainerCanWireEvents()
        {
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<SimpleEventBrokerExtension>();

            ClipboardManager clipboard = container.Resolve<ClipboardManager>();

            EventBroker broker = container.Configure<ISimpleEventBrokerConfiguration>().Broker;

            List<string> registeredEvents = new List<string>(broker.RegisteredEvents);
            registeredEvents.Sort();

            List<string> expectedEvents = new List<string>(new string[]
            {
                "cut",
                "copy",
                "paste",
                "clipboard data available"
            });
            expectedEvents.Sort();

            CollectionAssert.AreEqual(expectedEvents, registeredEvents);
        }
    }
}
