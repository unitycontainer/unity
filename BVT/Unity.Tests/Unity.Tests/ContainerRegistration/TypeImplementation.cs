// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity.Tests.ContainerRegistration
{
    internal class TypeImplementation : ITypeInterface
    {
        private string name;

        public TypeImplementation()
        {
        }

        public TypeImplementation(string name)
        {
            this.name = name;
        }

        #region ITypeInterface Members

        public string GetName()
        {
            return name;
        }

        #endregion
    }
}
