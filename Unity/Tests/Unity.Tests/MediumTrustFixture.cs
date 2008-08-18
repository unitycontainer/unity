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
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests
{
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
            partialTrustPermissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlEvidence));

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
