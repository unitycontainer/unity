// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.InterceptionExtension.Configuration.Tests.TestObjects
{
    public class Interceptable : MarshalByRefObject
    {
        public virtual int DoSomething()
        {
            return 10;
        }
    }
}
