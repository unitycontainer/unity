// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Tests
{
    [TestClass]
    public class BuildFailedExceptionFixture
    {
        [TestMethod]
        public void BuildExceptionFailureTest()
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup()
            {
                ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            };

            AppDomain newDomain = AppDomain.CreateDomain("Test Domain", null, appDomainSetup);

            AssertHelper.ThrowsException<ResolutionFailedException>(() => newDomain.DoCallBack(ExceptionThrowingCallback));
        }

        private static void ExceptionThrowingCallback()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                container.Resolve<IEnumerable<int>>();
            }
        }
    }
}
