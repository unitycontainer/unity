// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBuilder2.Tests.TestDoubles
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DependencyAttribute : Attribute
    {
    }
}
