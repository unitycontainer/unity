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
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Practices.Unity.Tests
{
    [TestClass]
    public class UnityContainerFixtureDesktop
    {
        [TestMethod]
        [Ignore]
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
        [Ignore]
        public void CanBuildUpComObject()
        {
            Type xmlHttpType = Type.GetTypeFromProgID("MSXML2.XMLHTTP.3.0");
            IUnknown xmlHttp = (IUnknown)Activator.CreateInstance(xmlHttpType);

            IUnknown o = (IUnknown)new UnityContainer().BuildUp(typeof(IUnknown), xmlHttp);

            Assert.IsNotNull(o);
        }

        [Guid("00000000-0000-0000-C000-000000000046")]
        public interface IUnknown
        {
        }


        [TestMethod]
        public void CanBuildUpProxiedClass()
        {
            InterfaceThroughProxy instance = (InterfaceThroughProxy)new MyProxy(typeof(InterfaceThroughProxy)).GetTransparentProxy();

            InterfaceThroughProxy o = (InterfaceThroughProxy)new UnityContainer().BuildUp(typeof(InterfaceThroughProxy), instance);
            
            Assert.IsNotNull(o);
        }
        
        public interface InterfaceThroughProxy
        {
        }

        public class MyProxy : RealProxy, IRemotingTypeInfo
        {
            Type t;
            public MyProxy(Type t) : base(t)
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
