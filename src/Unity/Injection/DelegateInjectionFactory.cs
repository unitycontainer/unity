using ObjectBuilder2;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Unity
{
    /// <summary>
    /// A class that lets you specify a factory method the container
    /// will use to create the object.
    /// </summary>
    /// <remarks>This factory allows to specify any signature for the method.</remarks>
    public class DelegateInjectionFactory : InjectionMember, IBuildPlanPolicy
    {
        #region Fields

        private readonly Delegate _factoryFunc;
        private readonly IEnumerable<ConstructorArgumentResolveOperation> _operations;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates Injection Factory with delegate as Factory method.
        /// 
        /// Delegate could have any signature. All of the dependencies will be 
        /// resolved either from container or with ParameterOverrides.
        /// The factory has two reserved argument names:
        ///   name - Always Name of the registration. Example RegisterType<T>("Name", ...
        ///   type - Always references type (T) of the registration.
        /// </summary>
        /// <param name="method"></param>
        public DelegateInjectionFactory(Delegate method)
        {
            _factoryFunc = method;
            _operations = null;
            //_operations = _factoryFunc.GetType().Method
            //                          .GetParameters()
            //                          .Select(p => new ConstructorArgumentResolveOperation(p.ParameterType, string.Empty, p.Name))
            //                          .ToArray();
        }

        #endregion


        #region InjectionMember

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            policies.Set<IBuildPlanPolicy>(this, new NamedTypeBuildKey(implementationType, name));
        }

        #endregion


        #region IBuildPlanPolicy

        public void BuildUp(IBuilderContext context)
        {
            if (context.Existing == null)
            {
                var container = context.NewBuildUp<IUnityContainer>();
                context.AddResolverOverrides(new ParameterOverride("type", new InjectionParameter(context.BuildKey.Type)));

                if (!string.IsNullOrWhiteSpace(context.BuildKey.Name))
                    context.AddResolverOverrides(new ParameterOverride("name", new InjectionParameter(context.BuildKey.Name)));

                context.Existing = _factoryFunc.DynamicInvoke(_operations.Select(p => ResolveArgument(p, context, container))
                                                                         .ToArray());

                DynamicMethodConstructorStrategy.SetPerBuildSingleton(context);
            }
        }

        private static object ResolveArgument(BuildOperation operation, IBuilderContext context, IUnityContainer container)
        {
            try
            {
                context.CurrentOperation = operation;
                var policy = context.GetOverriddenResolver(typeof(ParameterOverride));

                return null != policy
                    ? policy.Resolve(context)
                    : container.Resolve(operation.TypeBeingConstructed);
            }
            catch
            {
                // ignored
            }
            finally
            {
                context.CurrentOperation = null;
            }

            return operation.TypeBeingConstructed.GetDefaultValue();
        }

        #endregion
        /// <summary>
        /// Will return null for a reference type, or Activator.CreateInstance(t) for a value type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private object GetDefaultValue(this Type t)
        {
            if (t == null)
                return null;

            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}
