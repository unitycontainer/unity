// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class UnityContainerFixtureDesktop
    {
        [TestMethod]
        public void CanRegisterComObject()
        {
            Type xmlHttpType = Type.GetTypeFromProgID("MSXML2.XMLHTTP.3.0");
            IUnknown xmlHttp = (IUnknown)Activator.CreateInstance(xmlHttpType);

            IUnityContainer container = new UnityContainer()
                .RegisterInstance(typeof(IUnknown), "__comObject", xmlHttp);

            var o = container.Resolve<IUnknown>("__comObject");
            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void CanBuildUpComObject()
        {
            Type xmlHttpType = Type.GetTypeFromProgID("MSXML2.XMLHTTP.3.0");
            IUnknown xmlHttp = (IUnknown)Activator.CreateInstance(xmlHttpType);

            IUnknown o = (IUnknown)new UnityContainer().BuildUp(typeof(IUnknown), xmlHttp);

            Assert.IsNotNull(o);
        }

        [TestMethod]
        public void RegisteringComObjectForWrongInterfaceThrows()
        {
            Type xmlHttpType = Type.GetTypeFromProgID("MSXML2.XMLHTTP.3.0");
            var xmlHttp = Activator.CreateInstance(xmlHttpType);

            try
            {
                new UnityContainer().RegisterInstance(typeof(IEnumerable), "__comObject", xmlHttp);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException) { }
        }

        [Guid("00000000-0000-0000-C000-000000000046")]
        public interface IUnknown
        {
        }

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

        [TestMethod]
        public void RegisteringProxiedObjectForWrongInterfaceWhenProxyCannotAnswerGetTypeThrows()
        {
            var instance = (IService)new MyProxy(typeof(IService)).GetTransparentProxy();

            try
            {
                new UnityContainer().RegisterInstance(typeof(IEnumerable), "__proxy", instance);
                Assert.Fail("should have thrown");
            }
            catch (ArgumentException) { }
        }

        [ServiceContract]
        public interface IService
        {
            [OperationContract]
            void Ignore();
        }

        public class MyProxy : RealProxy, IRemotingTypeInfo
        {
            Type t;
            public MyProxy(Type t)
                : base(t)
            {
                this.t = t;
            }

            public override IMessage Invoke(IMessage msg)
            {
                return null;
            }

            public bool CanCastTo(Type fromType, object o)
            {
                return t.IsAssignableFrom(fromType);
            }

            public string TypeName
            {
                get
                {
                    return t.Name;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
