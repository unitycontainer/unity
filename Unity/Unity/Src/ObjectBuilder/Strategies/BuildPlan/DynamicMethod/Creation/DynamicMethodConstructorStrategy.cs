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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

        private static readonly MethodInfo setCurrentOperationToResolvingParameter =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToResolvingParameter(null, null, null));

        private static readonly MethodInfo setCurrentOperationToInvokingConstructor =
            StaticReflection.GetMethodInfo(() => SetCurrentOperationToInvokingConstructor(null, null));

        private static readonly MethodInfo setExistingInContext =
            StaticReflection.GetPropertySetMethodInfo<IBuilderContext, object>(ctx => ctx.Existing);

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

            IConstructorSelectorPolicy selector =
                context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey);

            SelectedConstructor selectedCtor = selector.SelectConstructor(context);

            GuardTypeIsNonPrimitive(context, selectedCtor);

            // Method preamble - test if we have an existing object
            // First off, set up jump - if there's an existing object, skip us entirely
            Label existingObjectNotNull = buildContext.IL.DefineLabel();

            if (!buildContext.TypeToBuild.IsValueType)
            {
                buildContext.EmitLoadExisting();
                buildContext.IL.Emit(OpCodes.Brtrue, existingObjectNotNull);
            }

            // Verify we're not attempting to create an instance of an interface
            buildContext.EmitLoadContext();
            buildContext.IL.EmitCall(OpCodes.Call, throwForAttemptingToConstructInterface, null);

            if (selectedCtor != null)
            {
                string signatureString = CreateSignatureString(selectedCtor.Constructor);

                // Resolve parameters
                ParameterInfo[] parameters = selectedCtor.Constructor.GetParameters();

                if (!parameters.Any(pi => pi.ParameterType.IsByRef))
                {
                    int i = 0;
                    foreach (string parameterKey in selectedCtor.GetParameterKeys())
                    {
                        // Set the current operation
                        buildContext.IL.Emit(OpCodes.Ldstr, parameters[i].Name);
                        buildContext.IL.Emit(OpCodes.Ldstr, signatureString);
                        buildContext.EmitLoadContext();
                        buildContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToResolvingParameter, null);

                        // Resolve the parameter
                        buildContext.EmitResolveDependency(parameters[i].ParameterType, parameterKey);
                        ++i;
                    }

                    // Set the current operation
                    buildContext.IL.Emit(OpCodes.Ldstr, signatureString);
                    buildContext.EmitLoadContext();
                    buildContext.IL.EmitCall(OpCodes.Call, setCurrentOperationToInvokingConstructor, null);

                    // Call the constructor
                    buildContext.IL.Emit(OpCodes.Newobj, selectedCtor.Constructor);

                    // Store the existing object
                    buildContext.EmitStoreExisting();

                    // Clear the current operation
                    buildContext.EmitClearCurrentOperation();

                    // Store existing object back into context - makes it available for future resolvers
                    buildContext.EmitLoadContext();
                    buildContext.EmitLoadExisting();
                    if (buildContext.TypeToBuild.IsValueType)
                    {
                        buildContext.IL.Emit(OpCodes.Box, buildContext.TypeToBuild);
                    }
                    buildContext.IL.EmitCall(OpCodes.Callvirt, setExistingInContext, null);

                    // Is this object a per-build singleton? If so, then emit code to stuff in
                    // the appropriate lifetime manager.
                    buildContext.EmitLoadContext();
                    buildContext.IL.EmitCall(OpCodes.Call, setPerBuildSingleton, null);
                    
                }
                else
                {
                    // if we get here the selected constructor has ref or out parameters.
                    buildContext.EmitLoadContext();
                    buildContext.IL.Emit(OpCodes.Ldstr, signatureString);
                    buildContext.IL.EmitCall(OpCodes.Call, throwForNullExistingObjectWithInvalidConstructor, null);
                }
            }
            else
            {
                // If we get here, object has no constructors. It's either
                // an interface or a primitive (like int). In this case,
                // verify that we have an Existing object, and if not,
                // throw (via helper function).
                buildContext.EmitLoadContext();
                buildContext.IL.EmitCall(OpCodes.Call, throwForNullExistingObject, null);
            }

            buildContext.IL.MarkLabel(existingObjectNotNull);
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
                              context.BuildKey.Type.Name));
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
                              context.BuildKey.Type.Name,
                              signature));
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
            if (context.BuildKey.Type.IsInterface)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.CannotConstructInterface,
                        context.BuildKey.Type,
                        context.BuildKey));
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
        /// A helper method used by the generated IL to set up a PerBuildSingleton lifetime manager
        /// if the current object is such.
        /// </summary>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetPerBuildSingleton(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            var lifetime = context.Policies.Get<ILifetimePolicy>(context.BuildKey);
            if(lifetime is PerBuildSingleton)
            {
                var perBuildLifetime = new PerBuildSingleton(context.Existing);
                context.Policies.Set<ILifetimePolicy>(perBuildLifetime, context.BuildKey);
            }
        }

        // Build up the string that will represent the constructor signature
        // in any exception message.
        private static string CreateSignatureString(ConstructorInfo ctor)
        {
            string typeName = ctor.DeclaringType.FullName;
            ParameterInfo[] parameters = ctor.GetParameters();
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
        private static void GuardTypeIsNonPrimitive(IBuilderContext context, SelectedConstructor selectedConstructor)
        {
            var typeToBuild = context.BuildKey.Type;
            if(!typeToBuild.IsInterface)
            {
                if(typeToBuild == typeof(string) || selectedConstructor == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.TypeIsNotConstructable,
                            typeToBuild.Name));
                }
            }
        }
    }
}
