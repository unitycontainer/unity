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
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.ObjectBuilder2
{
    /// <summary>
    /// This object tracks the current state of the build plan generation,
    /// accumulates the IL, provides the preamble &amp; postamble for the dynamic
    /// method, and tracks things like local variables in the generated IL
    /// so that they can be reused across IL generation strategies.
    /// </summary>
    public class DynamicBuildPlanGenerationContext
    {
        private readonly Type typeToBuild;
        private readonly DynamicMethod buildMethod;
        private readonly ILGenerator il;
        private LocalBuilder existingObjectLocal;

        private static readonly MethodInfo GetTypeFromHandle =
            StaticReflection.GetMethodInfo(() => Type.GetTypeFromHandle(typeof (Type).TypeHandle));

        private static readonly MethodInfo GetBuildKey =
            StaticReflection.GetPropertyGetMethodInfo((IBuilderContext ctx) => ctx.BuildKey);

        private static readonly MethodInfo GetExisting =
            StaticReflection.GetPropertyGetMethodInfo((IBuilderContext ctx) => ctx.Existing);

        private static readonly MethodInfo SetExisting =
            StaticReflection.GetPropertySetMethodInfo((IBuilderContext ctx) => ctx.Existing);

        private static readonly MethodInfo ResolveDependency =
            StaticReflection.GetMethodInfo((IDependencyResolverPolicy r) => r.Resolve(null));

        private static readonly MethodInfo GetResolverMethod =
            StaticReflection.GetMethodInfo(() => GetResolver(null, null, null));

        private static readonly MethodInfo ClearCurrentOperation =
            StaticReflection.GetMethodInfo(() => DoClearCurrentOperation(null));

        /// <summary>
        /// Create a <see cref="DynamicBuildPlanGenerationContext"/> that is initialized
        /// to handle creation of a dynamic method to build the given type.
        /// </summary>
        /// <param name="typeToBuild">Type that we're trying to create a build plan for.</param>
        /// <param name="builderMethodCreator">An <see cref="IDynamicBuilderMethodCreatorPolicy"/> object that actually
        /// creates our <see cref="DynamicMethod"/> object.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done via guard class")]
        public DynamicBuildPlanGenerationContext(Type typeToBuild, IDynamicBuilderMethodCreatorPolicy builderMethodCreator)
        {
            Guard.ArgumentNotNull(typeToBuild, "typeToBuild");
            Guard.ArgumentNotNull(builderMethodCreator, "builderMethodCreator");
            this.typeToBuild = typeToBuild;
            buildMethod = builderMethodCreator.CreateBuilderMethod(typeToBuild, BuildMethodName());
            il = buildMethod.GetILGenerator();
            CreatePreamble();
        }

        /// <summary>
        /// The underlying <see cref="ILGenerator"/> that can be used to
        /// emit IL into the generated dynamic method.
        /// </summary>
        public ILGenerator IL
        {
            get { return il; }
        }

        /// <summary>
        /// The type we're currently creating the method to build.
        /// </summary>
        public Type TypeToBuild
        {
            get { return typeToBuild; }
        }

        /// <summary>
        /// Completes generation of the dynamic method and returns the
        /// generated dynamic method delegate.
        /// </summary>
        /// <returns>The created <see cref="DynamicBuildPlanMethod"/></returns>
        internal DynamicBuildPlanMethod GetBuildMethod()
        {
            CreatePostamble();
            return (DynamicBuildPlanMethod)buildMethod.CreateDelegate(typeof(DynamicBuildPlanMethod));
        }

        #region IL Generation helper methods

        /// <summary>
        /// Emit the IL to put the build context on top of the IL stack.
        /// </summary>
        public void EmitLoadContext()
        {
            IL.Emit(OpCodes.Ldarg_0);
        }

        /// <summary>
        /// Emit the IL to put the current build key on top of the IL stack.
        /// </summary>
        public void EmitLoadBuildKey()
        {
            EmitLoadContext();
            IL.EmitCall(OpCodes.Callvirt, GetBuildKey, null);
        }

        /// <summary>
        /// Emit the IL to put the current "existing" object on the top of the IL stack.
        /// </summary>
        public void EmitLoadExisting()
        {
            IL.Emit(OpCodes.Ldloc, existingObjectLocal);
        }

        /// <summary>
        /// Emit the IL to make the top of the IL stack our current "existing" object.
        /// </summary>
        public void EmitStoreExisting()
        {
            IL.Emit(OpCodes.Stloc, existingObjectLocal);
        }

        /// <summary>
        /// Emit the IL to load the given <see cref="Type"/> object onto the top of the IL stack.
        /// </summary>
        /// <param name="t">Type to load on the stack.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t",
            Justification = "t is good enough - usage is pretty obvious")]
        public void EmitLoadTypeOnStack(Type t)
        {
            IL.Emit(OpCodes.Ldtoken, t);
            IL.EmitCall(OpCodes.Call, GetTypeFromHandle, null);
        }

        /// <summary>
        /// Emit the IL needed to look up an <see cref="IDependencyResolverPolicy"/> and
        /// call it to get a value.
        /// </summary>
        /// <param name="dependencyType">Type of the dependency to resolve.</param>
        /// <param name="key">Key to look up the policy by.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation is done via Guard class.")]
        public void EmitResolveDependency(Type dependencyType, string key)
        {
            Guard.ArgumentNotNull(dependencyType, "dependencyType");

            EmitLoadContext();
            EmitLoadTypeOnStack(dependencyType);
            IL.Emit(OpCodes.Ldstr, key);
            IL.EmitCall(OpCodes.Call, GetResolverMethod, null);
            EmitLoadContext();
            IL.EmitCall(OpCodes.Callvirt, ResolveDependency, null);
            EmitCastOrUnbox(dependencyType);
        }

        /// <summary>
        /// Emit the IL needed to clear the <see cref="IBuilderContext.CurrentOperation"/>.
        /// </summary>
        public void EmitClearCurrentOperation()
        {
            EmitLoadContext();
            IL.EmitCall(OpCodes.Call, ClearCurrentOperation, null);
        }

        /// <summary>
        /// Emit the IL needed to either cast the top of the stack to the target type
        /// or unbox it, if it's a value type.
        /// </summary>
        /// <param name="targetType">Type to convert the top of the stack to.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unbox", Justification="Unbox is spelled correctly.")]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CastOr", Justification = "Its two words, not one.")]
        public void EmitCastOrUnbox(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            if(targetType.IsValueType)
            {
                IL.Emit(OpCodes.Unbox_Any, targetType);
            }
            else
            {
                IL.Emit(OpCodes.Castclass, targetType);
            }
        }

        /// <summary>
        /// A helper method used by the generated IL to clear the current operation in the build context.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "Validation done by Guard class.")]
        public static void DoClearCurrentOperation(IBuilderContext context)
        {
            Guard.ArgumentNotNull(context, "context");

            context.CurrentOperation = null;
        }

        /// <summary>
        /// Helper method used by generated IL to look up a dependency resolver based on the given key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of the dependency being resolved.</param>
        /// <param name="resolverKey">Key the resolver was stored under.</param>
        /// <returns>The found dependency resolver.</returns>
        public static IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType, string resolverKey)
        {
            var resolver = context.GetOverriddenResolver(dependencyType);
            return resolver ?? context.Policies.Get<IDependencyResolverPolicy>(resolverKey);
        }

        /// <summary>
        /// A reflection helper method to make it easier to grab a property getter
        /// <see cref="MethodInfo"/> for the given property.
        /// </summary>
        /// <typeparam name="TImplementer">Type that implements the property we want.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>The property getter's <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Making this static results in very ugly calling code and doesn't actually improve perf.")]
        public MethodInfo GetPropertyGetter<TImplementer, TProperty>(string name)
        {
            return typeof(TImplementer).GetProperty(name, typeof(TProperty)).GetGetMethod();
        }

        /// <summary>
        /// A reflection helper method that makes it easier to grab a <see cref="MethodInfo"/>
        /// for a method.
        /// </summary>
        /// <typeparam name="TImplementer">Type that implements the method we want.</typeparam>
        /// <param name="name">Name of the method.</param>
        /// <param name="argumentTypes">Types of arguments to the method.</param>
        /// <returns>The method's <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
           Justification = "Making this static results in very ugly calling code and doesn't actually improve perf.")]
        public MethodInfo GetMethodInfo<TImplementer>(string name, params Type[] argumentTypes)
        {
            return typeof(TImplementer).GetMethod(name, argumentTypes);
        }

        #endregion

        private string BuildMethodName()
        {
            return "BuildUp_" + typeToBuild.FullName;
        }

        private void CreatePreamble()
        {
            existingObjectLocal = il.DeclareLocal(typeToBuild);
            EmitLoadContext();
            il.EmitCall(OpCodes.Callvirt, GetExisting, null);
            if(!typeToBuild.IsValueType)
            {
                il.Emit(OpCodes.Castclass, typeToBuild);
                EmitStoreExisting();
            }
            else
            {
                Label existingIsNullLabel = il.DefineLabel();
                Label doneLabel = il.DefineLabel();

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Beq_S, existingIsNullLabel);
                il.Emit(OpCodes.Unbox_Any, typeToBuild);
                EmitStoreExisting();
                il.Emit(OpCodes.Br_S, doneLabel);
                il.MarkLabel(existingIsNullLabel);
                il.Emit(OpCodes.Pop);
                il.MarkLabel(doneLabel);
            }
        }

        private void CreatePostamble()
        {
            EmitLoadContext();
            il.Emit(OpCodes.Ldloc, existingObjectLocal);
            if(typeToBuild.IsValueType)
            {
                il.Emit(OpCodes.Box, typeToBuild);
            }
            il.EmitCall(OpCodes.Callvirt, SetExisting, null);
            il.Emit(OpCodes.Ret);
        }

    }
}
