// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Properties;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that emits IL to call constructors
    /// as part of creating a build plan.
    /// </summary>
    public class DynamicMethodConstructorStrategy : BuilderStrategy
    {
        private static readonly MethodInfo throwForNullExistingObject =
          StaticReflection.GetMethodInfo(() => ThrowForNullExistingObject(null));

        private static readonly MethodInfo throwForNullExistingObjectWithInvalidConstructor =
            StaticReflection.GetMethodInfo(() => ThrowForNullExistingObjectWithInvalidConstructor(null, null));

        private static readonly MethodInfo throwForAttemptingToConstructInterface =
           StaticReflection.GetMethodInfo(() => ThrowForAttemptingToConstructInterface(null));

        private static readonly MethodInfo throwForAttemptingToConstructAbstractClass =
           StaticReflection.GetMethodInfo(() => ThrowForAttemptingToConstructAbstractClass(null));

        private static readonly MethodInfo throwForAttemptingToConstructDelegate =
           StaticReflection.GetMethodInfo(() => ThrowForAttemptingToConstructDelegate(null));

        private static readonly MethodInfo setCurrentOperationToResolvingParameter =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToResolvingParameter(null, null, null));

        private static readonly MethodInfo setCurrentOperationToInvokingConstructor =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToInvokingConstructor(null, null));

        private static readonly MethodInfo setPerBuildSingleton =
            StaticReflection.GetMethodInfo(() => SetPerBuildSingleton(null));

        /// <summary>
        /// Called during the chain of responsibility for a build operation.
        /// </summary>
        /// <remarks>Existing object is an instance of <see cref="DynamicBuildPlanGenerationContext"/>.</remarks>
        /// <param name="context">The context for the operation.</param>
        // FxCop suppression: Validation is done by Guard class
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public override void PreBuildUp(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            DynamicBuildPlanGenerationContext buildContext =
                (DynamicBuildPlanGenerationContext)context.Existing;

            GuardTypeIsNonPrimitive(context);

            buildContext.AddToBuildPlan(
                 Expression.IfThen(
                        Expression.Equal(
                            buildContext.GetExistingObjectExpression(),
                            Expression.Constant(null)),
                            CreateInstanceBuildupExpression(buildContext, context)));

            buildContext.AddToBuildPlan(
                Expression.Call(null, setPerBuildSingleton, buildContext.ContextParameter));
        }

        internal Expression CreateInstanceBuildupExpression(DynamicBuildPlanGenerationContext buildContext, IBuilderContext context)
        {
            var targetTypeInfo = context.BuildKey.Type.GetTypeInfo();

            if (targetTypeInfo.IsInterface)
            {
                return CreateThrowWithContext(buildContext, throwForAttemptingToConstructInterface);
            }

            if (targetTypeInfo.IsAbstract)
            {
                return CreateThrowWithContext(buildContext, throwForAttemptingToConstructAbstractClass);
            }

            if (targetTypeInfo.IsSubclassOf(typeof(Delegate)))
            {
                return CreateThrowWithContext(buildContext, throwForAttemptingToConstructDelegate);
            }

            IPolicyList resolverPolicyDestination;
            IConstructorSelectorPolicy selector =
                context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey, out resolverPolicyDestination);

            SelectedConstructor selectedConstructor = selector.SelectConstructor(context, resolverPolicyDestination);

            if (selectedConstructor == null)
            {
                return CreateThrowWithContext(buildContext, throwForNullExistingObject);
            }

            string signature = CreateSignatureString(selectedConstructor.Constructor);

            if (IsInvalidConstructor(selectedConstructor))
            {
                return CreateThrowForNullExistingObjectWithInvalidConstructor(buildContext, signature);
            }

            // psuedo-code:
            // throw if attempting interface
            // if (context.Existing == null) {
            //   collect parameters
            //   set operation to invoking constructor
            //   context.Existing = new {objectType}({constructorparameter}...)
            //   clear current operation
            // }
            return Expression.Block(CreateNewBuildupSequence(buildContext, selectedConstructor, signature));
        }

        private static bool IsInvalidConstructor(SelectedConstructor selectedConstructor)
        {
            return selectedConstructor.Constructor.GetParameters().Any(pi => pi.ParameterType.IsByRef);
        }

        private Expression CreateThrowWithContext(DynamicBuildPlanGenerationContext buildContext, MethodInfo throwMethod)
        {
            return Expression.Call(
                                null,
                                throwMethod,
                                buildContext.ContextParameter);
        }

        private Expression CreateThrowForNullExistingObjectWithInvalidConstructor(DynamicBuildPlanGenerationContext buildContext, string signature)
        {
            return Expression.Call(
                                null,
                                throwForNullExistingObjectWithInvalidConstructor,
                                buildContext.ContextParameter,
                                Expression.Constant(signature, typeof(string)));
        }

        private IEnumerable<Expression> CreateNewBuildupSequence(DynamicBuildPlanGenerationContext buildContext, SelectedConstructor selectedConstructor, string signature)
        {
            var parameterExpressions = BuildConstructionParameterExpressions(buildContext, selectedConstructor, signature);
            var newItemExpression = Expression.Variable(selectedConstructor.Constructor.DeclaringType, "newItem");


            yield return Expression.Call(null,
                                        setCurrentOperationToInvokingConstructor,
                                        Expression.Constant(signature),
                                        buildContext.ContextParameter
                                        );

            yield return Expression.Assign(
                            buildContext.GetExistingObjectExpression(),
                            Expression.Convert(
                                Expression.New(selectedConstructor.Constructor, parameterExpressions),
                                typeof(object)));

            yield return buildContext.GetClearCurrentOperationExpression();
        }

        private IEnumerable<Expression> BuildConstructionParameterExpressions(DynamicBuildPlanGenerationContext buildContext, SelectedConstructor selectedConstructor, string constructorSignature)
        {
            int i = 0;
            var constructionParameters = selectedConstructor.Constructor.GetParameters();

            foreach (IDependencyResolverPolicy parameterResolver in selectedConstructor.GetParameterResolvers())
            {
                yield return buildContext.CreateParameterExpression(
                                parameterResolver,
                                constructionParameters[i].ParameterType,
                                Expression.Call(null,
                                                setCurrentOperationToResolvingParameter,
                                                Expression.Constant(constructionParameters[i].Name, typeof(string)),
                                                Expression.Constant(constructorSignature),
                                                buildContext.ContextParameter
                                                )
                                );
                i++;
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to set up a PerResolveLifetimeManager lifetime manager
        /// if the current object is such.
        /// </summary>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetPerBuildSingleton(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            var lifetime = context.Policies.Get<ILifetimePolicy>(context.BuildKey);
            if (lifetime is PerResolveLifetimeManager)
            {
                var perBuildLifetime = new PerResolveLifetimeManager(context.Existing);
                context.Policies.Set<ILifetimePolicy>(perBuildLifetime, context.BuildKey);
            }
        }


        /// <summary>
        /// Build up the string that will represent the constructor signature
        /// in any exception message.
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Strategy should only ever expect constructor method")]
        public static string CreateSignatureString(ConstructorInfo constructor)
        {
            Guard.ArgumentNotNull(constructor, "constructor");

            string typeName = constructor.DeclaringType.FullName;
            ParameterInfo[] parameters = constructor.GetParameters();
            string[] parameterDescriptions = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                parameterDescriptions[i] = string.Format(CultureInfo.CurrentCulture,
                    "{0} {1}",
                    parameters[i].ParameterType.FullName,
                    parameters[i].Name);
            }

            return string.Format(CultureInfo.CurrentCulture,
                "{0}({1})",
                typeName,
                string.Join(", ", parameterDescriptions));

        }

        // Verify the type we're trying to build is actually constructable -
        // CLR primitive types like string and int aren't.
        private static void GuardTypeIsNonPrimitive(IBuilderContext context)
        {
            var typeToBuild = context.BuildKey.Type;
            if (!typeToBuild.GetTypeInfo().IsInterface)
            {
                if (typeToBuild == typeof(string))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.TypeIsNotConstructable,
                            typeToBuild.GetTypeInfo().Name));
                }
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToResolvingParameter(string parameterName, string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            context.CurrentOperation = new ConstructorArgumentResolveOperation(
                context.BuildKey.Type, constructorSignature, parameterName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToInvokingConstructor(string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            context.CurrentOperation = new InvokingConstructorOperation(
                context.BuildKey.Type, constructorSignature);
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an interface (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void ThrowForAttemptingToConstructInterface(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.CannotConstructInterface,
                    context.BuildKey.Type,
                    context.BuildKey));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an abstract class (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void ThrowForAttemptingToConstructAbstractClass(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.CannotConstructAbstractClass,
                    context.BuildKey.Type,
                    context.BuildKey));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an delegate other than Func{T} or Func{IEnumerable{T}}.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void ThrowForAttemptingToConstructDelegate(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.CannotConstructDelegate,
                    context.BuildKey.Type,
                    context.BuildKey));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static void ThrowForNullExistingObject(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                              Resources.NoConstructorFound,
                              context.BuildKey.Type.GetTypeInfo().Name));
        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// a dependency cannot be resolved because of an invalid constructor.
        /// </summary>
        /// <param name="context">The <see cref="IBuilderContext"/> currently being
        /// used for the build of this object.</param>
        /// <param name="signature">The signature of the invalid constructor.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class")]
        public static void ThrowForNullExistingObjectWithInvalidConstructor(IBuilderContext context, string signature)
        {
            Guard.ArgumentNotNull(context, "context");
            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                              Resources.SelectedConstructorHasRefParameters,
                              context.BuildKey.Type.GetTypeInfo().Name,
                              signature));
        }
    }
}
