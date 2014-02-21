// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Tests.ContainerRegistration
{
    internal class AnotherTypeImplementation : ITypeAnotherInterface
    {
        private readonly string name;

        public AnotherTypeImplementation()
        {
        }

        public AnotherTypeImplementation(string name)
        {
            this.name = name;
        }

        #region ITypeAnotherInterface Members

        public string GetName()
        {
            return name;
        }

        #endregion
    }
}
