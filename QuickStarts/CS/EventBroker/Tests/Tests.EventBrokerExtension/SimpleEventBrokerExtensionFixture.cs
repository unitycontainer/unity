//===============================================================================
// Microsoft patterns & practices
// Unity Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

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
