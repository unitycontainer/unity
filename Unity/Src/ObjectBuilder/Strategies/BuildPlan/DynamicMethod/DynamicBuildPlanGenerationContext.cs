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
using System.Security;
using System.Security.Permissions;

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
        private Type typeToBuild;
        private DynamicMethod buildMethod;
        private ILGenerator il;
        private LocalBuilder existingObjectLocal;

        private static readonly MethodInfo GetTypeFromHandle =
            typeof(Type).GetMethod("GetTypeFromHandle", Types(typeof(RuntimeTypeHandle)));

        private static readonly MethodInfo GetBuildKey =
            typeof(IBuilderContext).GetProperty("BuildKey").GetGetMethod();

        private static readonly MethodInfo GetExisting =
            typeof(IBuilderContext).GetProperty("Existing").GetGetMethod();

        private static readonly MethodInfo SetExisting =
            typeof(IBuilderContext).GetProperty("Existing").GetSetMethod();

        private static readonly MethodInfo GetPoliciesFromContext =
            typeof (IBuilderContext).GetProperty("Policies").GetGetMethod();

        private static readonly MethodInfo GetPolicy =
            typeof (IPolicyList).GetMethod("Get", Types(typeof (Type), typeof (object)));

        private static readonly MethodInfo ResolveDependency =
            typeof (IDependencyResolverPolicy).GetMethod("Resolve", Types(typeof(IBuilderContext)));

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
            Justification="Validation is done via Guard class.")]
        public void EmitResolveDependency(Type dependencyType, string key)
        {
            Guard.ArgumentNotNull(dependencyType, "dependencyType");

            EmitLoadContext();
            IL.EmitCall(OpCodes.Callvirt, GetPoliciesFromContext, null);
            EmitLoadTypeOnStack(typeof (IDependencyResolverPolicy));
            IL.Emit(OpCodes.Ldstr, key);
            IL.EmitCall(OpCodes.Callvirt, GetPolicy, null);
			IL.Emit(OpCodes.Castclass, typeof(IDependencyResolverPolicy));
			EmitLoadContext();
            IL.EmitCall(OpCodes.Callvirt, ResolveDependency, null);
			if (dependencyType.IsValueType)
			{
				IL.Emit(OpCodes.Unbox_Any, dependencyType);
			}
			else
			{
				IL.Emit(OpCodes.Castclass, dependencyType);
			}
		}

        /// <summary>
        /// A reflection helper method to make it easier to grab a property getter
        /// <see cref="MethodInfo"/> for the given property.
        /// </summary>
        /// <typeparam name="TImplementor">Type that implements the property we want.</typeparam>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>The property getter's <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification="Making this static results in very ugly calling code and doesn't actually improve perf.")]
        public MethodInfo GetPropertyGetter<TImplementor, TProperty>(string name)
        {
            return typeof(TImplementor).GetProperty(name, typeof(TProperty)).GetGetMethod();
        }

        /// <summary>
        /// A reflection helper method that makes it easier to grab a <see cref="MethodInfo"/>
        /// for a method.
        /// </summary>
        /// <typeparam name="TImplementor">Type that implements the method we want.</typeparam>
        /// <param name="name">Name of the method.</param>
        /// <param name="argumentTypes">Types of arguments to the method.</param>
        /// <returns>The method's <see cref="MethodInfo"/>.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
           Justification = "Making this static results in very ugly calling code and doesn't actually improve perf.")]
        public MethodInfo GetMethodInfo<TImplementor>(string name, params Type[] argumentTypes)
        {
            return typeof(TImplementor).GetMethod(name, argumentTypes);
        }

        #endregion

        private string BuildMethodName()
        {
            return "BuildUp_" + typeToBuild.FullName;
        }

        private static Type[] Types(params Type[] types)
        {
            return types;
        }

        private void CreatePreamble()
        {
            existingObjectLocal = il.DeclareLocal(typeToBuild);
			EmitLoadContext();
            il.EmitCall(OpCodes.Callvirt, GetExisting, null);
			il.Emit(OpCodes.Castclass, typeToBuild);
            EmitStoreExisting();
        }

        private void CreatePostamble()
        {
            EmitLoadContext();
            il.Emit(OpCodes.Ldloc, existingObjectLocal);
            il.EmitCall(OpCodes.Callvirt, SetExisting, null);
            il.Emit(OpCodes.Ret);
        }

    }
}
