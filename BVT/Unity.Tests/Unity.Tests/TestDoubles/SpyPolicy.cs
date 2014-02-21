// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;

namespace Unity.Tests
{
    internal class SpyPolicy : IBuilderPolicy
    {
        private bool wasSpiedOn;

        public bool WasSpiedOn
        {
            get { return wasSpiedOn; }
            set { wasSpiedOn = value; }
        }
    }
}
