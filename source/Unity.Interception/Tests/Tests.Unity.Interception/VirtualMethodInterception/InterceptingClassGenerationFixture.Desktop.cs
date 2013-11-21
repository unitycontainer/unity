// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.VirtualMethodInterception
{
    public partial class InterceptingClassGenerationFixture
    {
        [TestMethod]
        public void GeneratedTypeForAdditionalInterfaceWithMethodsHavingSignaturesMatchingMethodsInTheBaseClassIsVerifiable()
        {
            PermissionSet grantSet = new PermissionSet(PermissionState.None);
            grantSet.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution
                    | SecurityPermissionFlag.ControlEvidence
                    | SecurityPermissionFlag.ControlPolicy));
            grantSet.AddPermission(
                new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess
                    | ReflectionPermissionFlag.MemberAccess));
            grantSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));

            AppDomain sandbox =
                AppDomain.CreateDomain(
                    "sandbox",
                    AppDomain.CurrentDomain.Evidence,
                    new AppDomainSetup { ApplicationBase = AppDomain.CurrentDomain.BaseDirectory },
                    grantSet);

            sandbox.DoCallBack(() =>
            {
                InterceptingClassGenerator generator =
                    new InterceptingClassGenerator(typeof(MainType), typeof(IDoSomething), typeof(IDoSomethingToo));
                Type generatedType = generator.GenerateType();
            });
        }
    }
}
