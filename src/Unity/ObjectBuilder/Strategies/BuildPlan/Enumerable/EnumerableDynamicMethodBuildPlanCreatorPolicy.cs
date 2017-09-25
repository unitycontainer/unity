using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Utility;

namespace ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan for creating <see cref="IEnumerable{T}"/> objects.
    /// </summary>
    public class EnumerableDynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        private static readonly MethodInfo ResolveMethod = 
            typeof(EnumerableDynamicMethodBuildPlanCreatorPolicy).GetTypeInfo().DeclaredMethods
                .First(m => Equals(m.Name, nameof(EnumerableDynamicMethodBuildPlanCreatorPolicy.BuildResolveEnumerable)));

        private static readonly MethodInfo CastMethod = typeof(Enumerable).GetTypeInfo()
                                                                          .DeclaredMethods
                                                                          .First(m => Equals(m.Name, "Cast"));


        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(buildKey, "buildKey");

            var enumerableMethod = ResolveMethod.MakeGenericMethod(context.BuildKey.Type);
            var buildMethod = enumerableMethod.CreateDelegate(typeof(DynamicBuildPlanMethod));

            return new DynamicMethodBuildPlan((DynamicBuildPlanMethod)buildMethod);
        }

        private static void BuildResolveEnumerable<T>(IBuilderContext context)
        {
            if (null == context.Existing)
            {
                var key = context.BuildKey;
                var type = key.Type;
                var itemType = type.GetTypeInfo().GenericTypeArguments[0];

                var container = context.Container ?? context.NewBuildUp<IUnityContainer>();
                var list = container.Registrations.Where(r => Equals(itemType, r.RegisteredType)).Select(r => container.Resolve(r));
                context.Existing = CastMethod.MakeGenericMethod(itemType).Invoke(null, new[] { list });
            }

            // match the behavior of DynamicMethodConstructorStrategy
            DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
        }
    }
}
