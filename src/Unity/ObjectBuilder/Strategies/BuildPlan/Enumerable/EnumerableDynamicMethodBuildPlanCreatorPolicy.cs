using ObjectBuilder2;

namespace ObjectBuilder2
{
    /// <summary>
    /// An <see cref="IBuildPlanCreatorPolicy"/> implementation
    /// that constructs a build plan for creating <see cref="IEnumerable{T}"/> objects.
    /// </summary>
    public class EnumerableDynamicMethodBuildPlanCreatorPolicy : IBuildPlanCreatorPolicy
    {
        public IBuildPlanPolicy CreatePlan(IBuilderContext context, NamedTypeBuildKey buildKey)
        {
            return null;
        }
    }
}
