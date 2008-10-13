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