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
                              BuildKey.GetType(context.BuildKey).Name));
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
                              BuildKey.GetType(context.BuildKey).Name,
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
            if (BuildKey.GetType(context.BuildKey).IsInterface)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.CannotConstructInterface,
                        BuildKey.GetType(context.BuildKey),
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
                BuildKey.GetType(context.BuildKey), constructorSignature, parameterName);
        }

        /// <summary>
        /// A helper method used by the generated IL to store the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Validation done by Guard class.")]
        public static void SetCurrentOperationToInvokingConstructor(string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            context.CurrentOperation = new InvokingConstructorOperation(
                BuildKey.GetType(context.BuildKey), constructorSignature);
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
    }
}
