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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Properties;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class used to generate proxy classes for doing interception on
    /// interfaces.
    /// </summary>
    public partial class InterfaceInterceptorClassGenerator
    {
        private readonly Type typeToIntercept;
        private readonly IEnumerable<Type> additionalInterfaces;
        private static readonly AssemblyBuilder assemblyBuilder;

        private FieldBuilder proxyInterceptionPipelineField;
        private FieldBuilder targetField;
        private FieldBuilder typeToProxyField;
        private TypeBuilder typeBuilder;

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "Need to use constructor so we can place attribute on it.")]
        static InterfaceInterceptorClassGenerator()
        {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_InterfaceProxies"), 
#if DEBUG_SAVE_GENERATED_ASSEMBLY
                AssemblyBuilderAccess.RunAndSave
#else
                AssemblyBuilderAccess.Run
#endif
                );

        }

        /// <summary>
        /// Create an instance of <see cref="InterfaceInterceptorClassGenerator"/> that
        /// can construct an intercepting proxy for the given interface.
        /// </summary>
        /// <param name="typeToIntercept">Type of the interface to intercept.</param>
        /// <param name="additionalInterfaces">Additional interfaces the proxy must implement.</param>
        public InterfaceInterceptorClassGenerator(Type typeToIntercept, IEnumerable<Type> additionalInterfaces)
        {
            CheckAdditionalInterfaces(additionalInterfaces);

            this.typeToIntercept = typeToIntercept;
            this.additionalInterfaces = additionalInterfaces;
            CreateTypeBuilder();
        }

        private static void CheckAdditionalInterfaces(IEnumerable<Type> additionalInterfaces)
        {
            if (additionalInterfaces == null)
            {
                throw new ArgumentNullException("additionalInterfaces");
            }

            foreach (var type in additionalInterfaces)
            {
                if (type == null)
                {
                    throw new ArgumentException(
                        Resources.ExceptionContainsNullElement,
                        "additionalInterfaces");
                }
                if (!type.IsInterface)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, Resources.ExceptionTypeIsNotInterface, type.Name),
                        "additionalInterfaces");
                }
                if (type.IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, Resources.ExceptionTypeIsOpenGeneric, type.Name),
                        "additionalInterfaces");
                }
            }
        }

        /// <summary>
        /// Create the type to proxy the requested interface
        /// </summary>
        /// <returns></returns>
        public Type CreateProxyType()
        {
            HashSet<Type> implementedInterfaces = new HashSet<Type>();

            int memberCount =
                new InterfaceImplementation(
                    this.typeBuilder,
                    this.typeToIntercept,
                    this.proxyInterceptionPipelineField,
                    false,
                    this.targetField)
                    .Implement(implementedInterfaces, 0);

            foreach (var @interface in this.additionalInterfaces)
            {
                memberCount =
                    new InterfaceImplementation(
                        this.typeBuilder,
                        @interface,
                        this.proxyInterceptionPipelineField,
                        true)
                        .Implement(implementedInterfaces, memberCount);
            }

            AddConstructor();

            Type result = typeBuilder.CreateType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_InterfaceProxies.dll");
#endif
            return result;
        }

        private void AddConstructor()
        {
            Type[] paramTypes = Sequence.Collect(typeToIntercept, typeof(Type)).ToArray();

            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                paramTypes);

            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            ctorBuilder.DefineParameter(2, ParameterAttributes.None, "typeToProxy");
            ILGenerator il = ctorBuilder.GetILGenerator();

            // Call base class constructor

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectMethods.Constructor);

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, InterceptionBehaviorPipelineMethods.Constructor);
            il.Emit(OpCodes.Stfld, proxyInterceptionPipelineField);

            // Initialize the target field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, targetField);

            // Initialize the typeToProxy field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, typeToProxyField);

            il.Emit(OpCodes.Ret);
        }

        private void CreateTypeBuilder()
        {
            TypeAttributes newAttributes = TypeAttributes.Public | TypeAttributes.Class;

            ModuleBuilder moduleBuilder = GetModuleBuilder();
            typeBuilder = moduleBuilder.DefineType(CreateTypeName(), newAttributes);

            DefineGenericArguments();

            proxyInterceptionPipelineField = InterceptingProxyImplementor.ImplementIInterceptingProxy(typeBuilder);
            targetField = typeBuilder.DefineField("target", typeToIntercept, FieldAttributes.Private);
            typeToProxyField = typeBuilder.DefineField("typeToProxy", typeof(Type), FieldAttributes.Private);
        }


        private string CreateTypeName()
        {
            return "DynamicModule.ns.Wrapped_" + typeToIntercept.Name + "_" + Guid.NewGuid().ToString("N");
        }

        private void DefineGenericArguments()
        {
            if (!typeToIntercept.IsGenericType) return;

            Type[] genericArguments = typeToIntercept.GetGenericArguments();

            GenericTypeParameterBuilder[] genericTypes = typeBuilder.DefineGenericParameters(
                genericArguments.Select(t => t.Name).ToArray());

            for (int i = 0; i < genericArguments.Length; ++i)
            {
                genericTypes[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);
                foreach (Type constraint in genericArguments[i].GetGenericParameterConstraints())
                {
                    genericTypes[i].SetBaseTypeConstraint(constraint);
                }
            }
        }
    }
}
