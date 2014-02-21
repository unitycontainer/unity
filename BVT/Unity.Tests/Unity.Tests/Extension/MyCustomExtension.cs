// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Unity.Tests.Extension
{
    internal class MyCustomExtension : UnityContainerExtension, IMyCustomConfigurator
    {
        private bool checkExtensionAdded = false;
        private bool checkPolicyAdded = false;

        public bool CheckExtensionValue
        {
            get { return this.checkExtensionAdded; }
            set { this.checkExtensionAdded = value; }
        }

        public bool CheckPolicyValue
        {
            get { return this.checkPolicyAdded; }
            set { this.checkPolicyAdded = value; }
        }

        protected override void Initialize()
        {
            this.checkExtensionAdded = true;
            this.AddPolicy();
        }

        public IMyCustomConfigurator AddPolicy()
        {
            Context.Policies.Set<IBuildPlanPolicy>(new MyCustomPolicy(), null);
            this.checkPolicyAdded = true;
            return this;
        }
    }
}
