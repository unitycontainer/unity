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
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.ObjectBuilder2.Properties;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// A <see cref="BuilderStrategy"/> that emits IL to call constructors
    /// as part of creating a build plan.
    /// </summary>
    public class DynamicMethodConstructorStrategy : BuilderStrategy
    {
        private static MethodInfo throwForNullExistingObject =
            typeof (DynamicMethodConstructorStrategy).GetMethod("ThrowForNullExistingObject");

        private static MethodInfo throwForResolutionFailed =
            typeof(DynamicMethodConstructorStrategy).GetMethod("ThrowForResolutionFailed");

        private static MethodInfo throwForAttemptingToConstructInterface =
            typeof(DynamicMethodConstructorStrategy).GetMethod(
                "ThrowForAttemptingToConstructInterface");

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
            buildContext.EmitLoadExisting();
            buildContext.IL.Emit(OpCodes.Ldnull);
            buildContext.IL.Emit(OpCodes.Ceq);
            buildContext.IL.Emit(OpCodes.Brfalse, existingObjectNotNull);

            // Verify we're not attempting to create an instance of an interface
            buildContext.EmitLoadContext();
            buildContext.IL.EmitCall(OpCodes.Call, throwForAttemptingToConstructInterface, null);

            if (selectedCtor != null)
            {
                string signatureString = CreateSignatureString(selectedCtor.Constructor);
                LocalBuilder currentParameterName = buildContext.IL.DeclareLocal(typeof(string));

                // Resolve parameters
                ParameterInfo[] parameters = selectedCtor.Constructor.GetParameters();

                buildContext.IL.BeginExceptionBlock();
                
                int i = 0;
                foreach (string parameterKey in selectedCtor.GetParameterKeys())
                {
                    buildContext.IL.Emit(OpCodes.Ldstr, parameters[i].Name);
                    buildContext.IL.Emit(OpCodes.Stloc, currentParameterName);
                    buildContext.EmitResolveDependency(parameters[i].ParameterType, parameterKey);
                    ++i;
                }

                // Call the constructor

                buildContext.IL.Emit(OpCodes.Ldnull);
                buildContext.IL.Emit(OpCodes.Stloc, currentParameterName);

                buildContext.IL.Emit(OpCodes.Newobj, selectedCtor.Constructor);
                buildContext.EmitStoreExisting();

                buildContext.IL.BeginCatchBlock(typeof(Exception));
                Label exceptionOccuredInResolution = buildContext.IL.DefineLabel();
                
                buildContext.IL.Emit(OpCodes.Ldloc, currentParameterName);
                buildContext.IL.Emit(OpCodes.Ldnull);
                buildContext.IL.Emit(OpCodes.Ceq);
                buildContext.IL.Emit(OpCodes.Brfalse, exceptionOccuredInResolution);
                buildContext.IL.Emit(OpCodes.Rethrow);

                buildContext.IL.MarkLabel(exceptionOccuredInResolution);
                buildContext.IL.Emit(OpCodes.Ldloc, currentParameterName);
                buildContext.IL.Emit(OpCodes.Ldstr, signatureString);
                buildContext.EmitLoadContext();
                buildContext.IL.EmitCall(OpCodes.Call, throwForResolutionFailed, null);
                
                buildContext.IL.EndExceptionBlock();
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
        /// A helper method used by the generated IL to throw an exception if a parameter
        /// fails to resolve.
        /// </summary>
        /// <param name="inner">Inner exception to throw.</param>
        /// <param name="parameterName">Name of the parameter that was attempted to be resolved.</param>
        /// <param name="constructorSignature">String describing which constructor we were calling.</param>
        /// <param name="context">Current build context.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification="Validation done by Guard class.")]
        public static void ThrowForResolutionFailed(Exception inner, string parameterName, string constructorSignature, IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            throw new InvalidOperationException(
                string.Format(CultureInfo.CurrentCulture,
                    Resources.ConstructorParameterResolutionFailed,
                    parameterName,
                    constructorSignature), inner);

        }

        /// <summary>
        /// A helper method used by the generated IL to throw an exception if
        /// no existing object is present, but the user is attempting to build
        /// an interface (usually due to the lack of a type mapping).
        /// </summary>
        /// <param name="context"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification="Validation done by Guard class.")]
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

        // Build up the string that will represent the constructor signature
        // in any exception message.
        private static string CreateSignatureString(ConstructorInfo ctor)
        {
            string typeName = ctor.DeclaringType.FullName;
            ParameterInfo[] parameters = ctor.GetParameters();
            string[] parameterDescriptions = new string[parameters.Length];
            for(int i = 0; i < parameters.Length; ++i)
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
