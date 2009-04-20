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
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class PipelineManagerMethods
    {
        internal static ConstructorInfo Constructor { get { return typeof (PipelineManager).GetConstructor(new Type[0]); } }
        internal static MethodInfo GetPipeline { get { return typeof(PipelineManager).GetMethod("GetPipeline"); } }
        internal static MethodInfo SetPipeline { get { return typeof(PipelineManager).GetMethod("SetPipeline"); } }
    }
}
