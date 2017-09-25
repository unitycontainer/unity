using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Properties;
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

                if (itemType.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                Resources.MustHaveOpenGenericType,
                                                itemType.GetTypeInfo().Name));
                }

                IEnumerable<object> enumerable;
                if (itemType.IsGenericType)
                {
                    var generic = new Lazy<Type>(() => itemType.GetGenericTypeDefinition());
                    enumerable = container.Registrations
                                          .Where(r => r.RegisteredType == itemType || 
                                                     (r.RegisteredType.IsGenericTypeDefinition && 
                                                      r.RegisteredType == generic.Value))
                                          .Select(r => context.NewBuildUp(new NamedTypeBuildKey(itemType, r.Name)));
                }
                else
                {
                    enumerable = container.Registrations.Where(r => r.RegisteredType == itemType).Select(r => container.Resolve(r));
                }

                context.Existing = CastMethod.MakeGenericMethod(itemType).Invoke(null, new object[] { enumerable });
                context.BuildComplete = true;
            }

            // match the behavior of DynamicMethodConstructorStrategy
            DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
        }
    }
}
