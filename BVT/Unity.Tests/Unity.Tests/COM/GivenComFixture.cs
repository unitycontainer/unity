// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests.COM
{
    /// <summary>
    /// dao.IndexClass => 00000105-0000-0010-8000-00AA006D2EA4
    /// </summary>
    [TestClass]
    public class GivenComFixture
    {
        [TestMethod]
        public void WhenComObjectIsRegisteredAsInstance()
        {
            var unity = new UnityContainer();

            Type type = Type.GetTypeFromCLSID(new Guid("00000105-0000-0010-8000-00AA006D2EA4"), true);

            IUknown daoIndexClass = (IUknown)Activator.CreateInstance(type);

            Assert.IsNotNull(daoIndexClass);

            unity.RegisterInstance<IUknown>("daoIndexClass", daoIndexClass);

            var result = unity.Resolve<IUknown>("daoIndexClass");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenComObjectIsBuiltUsingContainer()
        {
            var unity = new UnityContainer();

            Type type = Type.GetTypeFromCLSID(new Guid("00000105-0000-0010-8000-00AA006D2EA4"), true);

            IUknown daoIndexClass = (IUknown)Activator.CreateInstance(type);

            Assert.IsNotNull(daoIndexClass);

            unity.BuildUp<IUknown>(daoIndexClass);
        }
    }

    [ComImport, Guid("00000000-0000-0000-C000-000000000046")]
    public interface IUknown
    { }
}
