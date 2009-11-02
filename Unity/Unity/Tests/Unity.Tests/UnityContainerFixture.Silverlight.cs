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
using System.Collections;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class UnityContainerFixtureDesktop
    {
        [TestMethod]
        public void CanBuildUpProxiedClass()
        {
            using (var channelFactory = GetChannelFactory())
            {
                var client = new UnityContainer().BuildUp<IService>(channelFactory.CreateChannel());
                Assert.IsNotNull(client);
            }
        }

        [TestMethod]
        public void RegisteringProxiedObjectForWrongInterfaceThrows()
        {
            using (var channelFactory = GetChannelFactory())
            {
                var client = channelFactory.CreateChannel();

                try
                {
                    new UnityContainer().RegisterInstance(typeof(IEnumerable), "__wcfObject", client);
                    Assert.Fail("should have thrown");
                }
                catch (ArgumentException) { }
            }
        }

        private static ChannelFactory<IService> GetChannelFactory()
        {
            return
                new ChannelFactory<IService>(
                    new BasicHttpBinding(),
                    new EndpointAddress(@"http://www.fabrikam.com:322/mathservice.svc/secureEndpoint"));
        }

        [ServiceContract]
        public interface IService
        {
            [OperationContract]
            void Ignore();
        }
    }
}
