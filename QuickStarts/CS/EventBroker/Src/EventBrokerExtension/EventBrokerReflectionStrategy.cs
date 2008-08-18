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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using SimpleEventBroker;

namespace EventBrokerExtension
{
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context.Policies.Get<IEventBrokerInfoPolicy>(context.BuildKey) == null)
            {
                EventBrokerInfoPolicy policy = new EventBrokerInfoPolicy();
                context.Policies.Set<IEventBrokerInfoPolicy>(policy, context.BuildKey);

                AddPublicationsToPolicy(context.BuildKey, policy);
                AddSubscriptionsToPolicy(context.BuildKey, policy);
            }
        }

        private void AddPublicationsToPolicy(object buildKey, EventBrokerInfoPolicy policy)
        {
            Type t = BuildKey.GetType(buildKey);
            foreach(EventInfo eventInfo in t.GetEvents())
            {
                PublishesAttribute[] attrs =
                    (PublishesAttribute[])eventInfo.GetCustomAttributes(typeof(PublishesAttribute), true);
                if(attrs.Length > 0)
                {
                    policy.AddPublication(attrs[0].EventName, eventInfo.Name);
                }
            }
        }

        private void AddSubscriptionsToPolicy(object buildKey, EventBrokerInfoPolicy policy)
        {
            Type t = BuildKey.GetType(buildKey);
            foreach(MethodInfo method in t.GetMethods())
            {
                SubscribesToAttribute[] attrs =
                    (SubscribesToAttribute[])
                    method.GetCustomAttributes(typeof(SubscribesToAttribute), true);
                if(attrs.Length > 0)
                {
                    policy.AddSubscription(attrs[0].EventName, method);
                }
            }
        }
    }
}
