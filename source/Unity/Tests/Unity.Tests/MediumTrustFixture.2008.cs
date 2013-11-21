// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
    // This test uses security features that have been deprecated in .NET 4.0, so is
    // only included in the VS 2008 projects.
    [TestClass]
    public class MediumTrustFixture
    {
        [TestMethod]
        public void GeneratesVerifiableIL()
        {
            AppDomain spawnedDomain = CreateRestrictedDomain("spawned");
            try
            {
                UnityProxy proxy = (UnityProxy)spawnedDomain.CreateInstanceAndUnwrap(typeof(UnityProxy).Assembly.FullName, typeof(UnityProxy).FullName);
                Assert.IsTrue(proxy.CreateSomething());
            }
            catch(Exception ex)
            {
                Assert.Fail(string.Format("Exception of type {0} was caught, message {1}", ex.GetType(), ex.Message));
            }
            finally
            {
                AppDomain.Unload(spawnedDomain);
            }
        }

        #region AppDomain Spawn helper

        private static AppDomain CreateRestrictedDomain(string domainName)
        {
            // Default to all code getting nothing
            PolicyStatement emptyPolicy = new PolicyStatement(new PermissionSet(PermissionState.None));
            UnionCodeGroup policyRoot = new UnionCodeGroup(new AllMembershipCondition(), emptyPolicy);

            // Grant all code the named permission set for the test
            PermissionSet partialTrustPermissionSet = new PermissionSet(PermissionState.None);
            partialTrustPermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.AllFlags));
            partialTrustPermissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy));

            PolicyStatement permissions = new PolicyStatement(partialTrustPermissionSet);
            policyRoot.AddChild(new UnionCodeGroup(new AllMembershipCondition(), permissions));

            // Create an AppDomain policy level for the policy tree
            PolicyLevel appDomainLevel = PolicyLevel.CreateAppDomainLevel();
            appDomainLevel.RootCodeGroup = policyRoot;

            // Set the Application Base correctly in order to find the test assembly
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = Environment.CurrentDirectory;

            AppDomain restrictedDomain = AppDomain.CreateDomain(domainName, null, ads);
            restrictedDomain.SetAppDomainPolicy(appDomainLevel);

            return restrictedDomain;
        }

        #endregion
    }
    
    internal class UnityProxy : MarshalByRefObject
    {
        public bool CreateSomething()
        {
            try
            {
                IUnityContainer container = new UnityContainer();
                MockClass mock = container.Resolve<MockClass>();

                return mock != null;
            }
            catch 
            {
                return false;
            }
        }
    }

    public class MockClass
    {
        private string data;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
