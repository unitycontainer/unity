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

using System;
using System.Collections.Generic;
using EventBrokerExtension;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEventBroker;
using Tests.EventBrokerExtension.TestObjects;
using Tests.EventBrokerExtension.Utility;

namespace Tests.EventBrokerExtension
{
    [TestClass]
    public class EventBrokerWireupStrategyFixture
    {
        [TestMethod]
        public void StrategyThrowsIfWireupIsNeededAndBrokerNotInLocator()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<OneEventPublisher>();
            EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
            policy.AddPublication("paste", "Pasting");
            context.Policies.Set<IEventBrokerInfoPolicy>(policy, buildKey);

            try
            {
                OneEventPublisher existing = new OneEventPublisher();
                context.ExecuteBuildUp(buildKey, existing);
            }
            catch (Exception)
            {
                // If we got here, we're Ok, this is expected.
                return;
            }
            Assert.Fail("No exception occured");
        }

        [TestMethod]
        public void NoExceptionIfExistingObjectDoesntAndNoBroker()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<OneEventPublisher>();
            EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
            policy.AddPublication("paste", "Pasting");
            context.Policies.Set<IEventBrokerInfoPolicy>(policy, buildKey);

            context.ExecuteBuildUp(buildKey, null);
            // No assert needed, if we got here, we're ok            
        }

        [TestMethod]
        public void ExceptionIfNoWireupNeededAndNoBroker()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<object>();
            EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
            context.Policies.Set<IEventBrokerInfoPolicy>(policy, buildKey);

            try
            {
                context.ExecuteBuildUp(buildKey, new object());
            }
            catch (Exception)
            {
                // If we got here, we're ok            
                return;
            }
            Assert.Fail("No exception Occurred");
        }

        [TestMethod]
        public void StrategyProperlyWiresEvents()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<ClipboardManager>();

            EventBroker broker = new EventBroker();
            var brokerLifetime = new ExternallyControlledLifetimeManager();
            brokerLifetime.SetValue(broker);
            context.Policies.Set<ILifetimePolicy>(brokerLifetime, NamedTypeBuildKey.Make<EventBroker>());

            EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
            policy.AddPublication("cut", "Cut");
            policy.AddPublication("copy", "Copy");
            policy.AddPublication("paste", "Paste");

            policy.AddSubscription("copy", typeof(ClipboardManager).GetMethod("OnCopy"));
            policy.AddSubscription("clipboard data available",
                                   typeof(ClipboardManager).GetMethod("OnClipboardDataAvailable"));

            context.Policies.Set<IEventBrokerInfoPolicy>(policy, buildKey);

            ClipboardManager existing = new ClipboardManager();

            context.ExecuteBuildUp(buildKey, existing);

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

        private MockBuilderContext CreateContext()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Strategies.Add(new LifetimeStrategy());
            context.Strategies.Add(new EventBrokerWireupStrategy());

            return context;
        }
    }
}
