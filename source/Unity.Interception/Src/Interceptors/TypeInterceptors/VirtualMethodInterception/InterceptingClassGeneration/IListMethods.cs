// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Collections;
using System.Reflection;

namespace Unity.InterceptionExtension
{
    internal static class IListMethods
    {
        internal static MethodInfo GetItem
        {
            get { return typeof(IList).GetProperty("Item").GetGetMethod(); }
        }
    }
}
