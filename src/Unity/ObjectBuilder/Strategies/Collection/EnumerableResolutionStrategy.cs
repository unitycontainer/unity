using System;
using System.Reflection;
using ObjectBuilder2;
using System.Collections.Generic;

namespace Unity
{
    /// <summary>
    /// This strategy implements the logic that will arrange resolution
    /// when an IEnumerable<T> type is detected.
    /// </summary>
    public class EnumerableResolutionStrategy : BuilderStrategy
    {
        private static Type enumerable = typeof(IEnumerable<>);

        public override void PreBuildUp(IBuilderContext context)
        {
            var info = context.BuildKey.Type.GetTypeInfo();
            if (!info.IsInterface || !info.IsGenericType || 
                !Equals(enumerable, context.BuildKey.Type.GetGenericTypeDefinition()))
                return;

            System.Diagnostics.Debug.WriteLine("");
        }
    }
}
