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

using System.Collections;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IListMethods
    {
        internal static MethodInfo GetItem
        {
            get { return typeof(IList).GetProperty("Item").GetGetMethod(); }
        }
    }
}
