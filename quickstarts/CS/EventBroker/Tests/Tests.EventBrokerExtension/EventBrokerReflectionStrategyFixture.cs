// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using EventBrokerExtension;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.EventBrokerExtension.TestObjects;
using Tests.EventBrokerExtension.Utility;

namespace Tests.EventBrokerExtension
{
    /// <summary>
    /// Tests of the strategies that looks at objects and
    /// reflects over them looking for event subscriptions.
    /// </summary>
    [TestClass]
    public class EventBrokerReflectionStrategyFixture
    {
        [TestMethod]
        public void ReflectingOverObjectWithoutSubscriptionResultsInEmptyPolicy()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<object>();

            context.ExecuteBuildUp(buildKey, null);

            IEventBrokerInfoPolicy policy = context.Policies.Get<IEventBrokerInfoPolicy>(buildKey);
            Assert.IsNotNull(policy);

            List<PublicationInfo> publications = new List<PublicationInfo>(policy.Publications);
            List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>(policy.Subscriptions);

            Assert.AreEqual(0, publications.Count);
            Assert.AreEqual(0, subscriptions.Count);
        }

        [TestMethod]
        public void StrategyDoesntOverwriteAnExistingPolicy()
        {
            MockBuilderContext context = CreateContext();

            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<object>();
            EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
            context.Policies.Set<IEventBrokerInfoPolicy>(policy, buildKey);

            context.ExecuteBuildUp(buildKey, null);

            IEventBrokerInfoPolicy setPolicy =
                context.Policies.Get<IEventBrokerInfoPolicy>(buildKey);
            Assert.AreSame(policy, setPolicy);
        }

        [TestMethod]
        public void ReflectingOverPublishingTypeResultsInCorrectPolicy()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<OneEventPublisher>();

            context.ExecuteBuildUp(buildKey, null);

            IEventBrokerInfoPolicy policy = context.Policies.Get<IEventBrokerInfoPolicy>(buildKey);

            Assert.IsNotNull(policy);

            List<PublicationInfo> publications = new List<PublicationInfo>(policy.Publications);
            List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>(policy.Subscriptions);

            Assert.AreEqual(0, subscriptions.Count);

            CollectionAssert.AreEqual(new PublicationInfo[] { new PublicationInfo("paste", "Pasting") }, publications);
        }

        [TestMethod]
        public void ReflectingOverSubscribingTypeResultsInCorrectPolicy()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<OneEventSubscriber>();

            context.ExecuteBuildUp(buildKey, null);

            IEventBrokerInfoPolicy policy = context.Policies.Get<IEventBrokerInfoPolicy>(buildKey);

            Assert.IsNotNull(policy);

            List<PublicationInfo> publications = new List<PublicationInfo>(policy.Publications);
            List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>(policy.Subscriptions);

            Assert.AreEqual(0, publications.Count);

            CollectionAssert.AreEqual(new SubscriptionInfo[] { new SubscriptionInfo("copy", typeof(OneEventSubscriber).GetMethod("OnCopy")) }, subscriptions);
        }

        [TestMethod]
        public void OneTypeCanPublishAndSubscribeMultipleTimes()
        {
            MockBuilderContext context = CreateContext();
            NamedTypeBuildKey buildKey = NamedTypeBuildKey.Make<ClipboardManager>();

            context.ExecuteBuildUp(buildKey, null);

            IEventBrokerInfoPolicy policy = context.Policies.Get<IEventBrokerInfoPolicy>(buildKey);

            Assert.IsNotNull(policy);

            List<PublicationInfo> publications = new List<PublicationInfo>(policy.Publications);
            List<SubscriptionInfo> subscriptions = new List<SubscriptionInfo>(policy.Subscriptions);

            publications.Sort(
                delegate(PublicationInfo a, PublicationInfo b)
                {
                    return a.PublishedEventName.CompareTo(b.PublishedEventName);
                });

            subscriptions.Sort(
                delegate(SubscriptionInfo a, SubscriptionInfo b)
                {
                    return a.PublishedEventName.CompareTo(b.PublishedEventName);
                });

            CollectionAssert.AreEqual(
                new PublicationInfo[]
                {
                    new PublicationInfo("copy", "Copy"),
                    new PublicationInfo("cut", "Cut"),
                    new PublicationInfo("paste", "Paste"),
                },
                publications);

            CollectionAssert.AreEqual(
                new SubscriptionInfo[]
                {
                    new SubscriptionInfo("clipboard data available", typeof(ClipboardManager).GetMethod("OnClipboardDataAvailable")), 
                    new SubscriptionInfo("copy", typeof(ClipboardManager).GetMethod("OnCopy")),
                },
                subscriptions);
        }


        private MockBuilderContext CreateContext()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Strategies.Add(new EventBrokerReflectionStrategy());

            return context;
        }
    }
}
