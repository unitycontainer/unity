// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Security;
using System.Security.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.InterceptionExtension.Tests.VirtualMethodInterceptorTests
{
    public partial class VirtualMethodInterceptorFixture
    {
        [TestMethod]
        public void GeneratedTypeForAbstractIsVerifiable()
        {
            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution
                    | SecurityPermissionFlag.ControlPolicy
                    | SecurityPermissionFlag.ControlPrincipal));
            permissionSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));

            AppDomain domain =
                AppDomain.CreateDomain(
                    "isolated",
                    AppDomain.CurrentDomain.Evidence,
                    AppDomain.CurrentDomain.SetupInformation,
                    permissionSet);

            DerivedTypeCreator creator = (DerivedTypeCreator)
                domain.CreateInstanceAndUnwrap(
                    typeof(DerivedTypeCreator).Assembly.FullName,
                    typeof(DerivedTypeCreator).FullName);

            creator.CreateType(typeof(AbstractClassWithPublicConstructor));
        }
    }

    public partial class DerivedTypeCreator : MarshalByRefObject
    {
    }
}
