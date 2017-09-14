// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.PolicyInjection
{
    public partial class PolicyInjectionWithGenericMethodsFixture
    {
        [TestMethod]
        public void TransparentProxyCanInterceptNonGenericMethod()
        {
            CanInterceptNonGenericMethod<TransparentProxyInterceptor>();
        }

        [TestMethod]
        public void TransparentProxyCanInterceptGenericMethod()
        {
            CanInterceptGenericMethod<TransparentProxyInterceptor>();
        }
    }
}
