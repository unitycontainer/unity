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
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity.InterceptionExtension.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A class used to generate proxy classes for doing interception on
    /// interfaces.
    /// </summary>
    public class InterfaceInterceptorClassGenerator
    {
        private readonly Type typeToIntercept;
        private static readonly AssemblyBuilder assemblyBuilder;

        private FieldBuilder pipelineManagerField;
        private FieldBuilder targetField;
        private TypeBuilder typeBuilder;

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "Need to use constructor so we can place attribute on it.")]
        static InterfaceInterceptorClassGenerator()
        {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_InterfaceProxies"), AssemblyBuilderAccess.RunAndSave);

        }

        /// <summary>
        /// Create an instance of <see cref="InterfaceInterceptorClassGenerator"/> that
        /// can construct an intercepting proxy for the given interface.
        /// </summary>
        /// <param name="typeToIntercept">Type of the interface to intercept.</param>
        public InterfaceInterceptorClassGenerator(Type typeToIntercept)
        {
            this.typeToIntercept = typeToIntercept;
            CreateTypeBuilder();
        }

        /// <summary>
        /// Create the type to proxy the requested interface
        /// </summary>
        /// <returns></returns>
        public Type CreateProxyType()
        {
            int memberCount = 0;
            foreach(MethodInfo method in MethodsToIntercept())
            {
                OverrideMethod(method, memberCount++);
            }

            foreach(PropertyInfo property in PropertiesToIntercept())
            {
                OverrideProperty(property, memberCount++);
            }

            AddConstructor();

            Type result = typeBuilder.CreateType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_InterfaceProxies.dll");
#endif
            return result;
        }

        private IEnumerable<MethodInfo> MethodsToIntercept()
        {
            foreach(MethodInfo method in typeToIntercept.GetMethods())
            {
                if(!method.IsSpecialName)
                {
                    yield return method;
                }
            }
        }

        private void OverrideMethod(MethodInfo method, int methodNum)
        {
            new InterfaceMethodOverride(typeBuilder, pipelineManagerField, targetField, method, methodNum).AddMethod();
        }

        private IEnumerable<PropertyInfo> PropertiesToIntercept()
        {
            return typeToIntercept.GetProperties();
        }

        private void OverrideProperty(PropertyInfo property, int count)
        {
            MethodBuilder getMethod = OverridePropertyMethod(property.GetGetMethod(), count);
            MethodBuilder setMethod = OverridePropertyMethod(property.GetSetMethod(), count);
            AddPropertyDefinition(property, getMethod, setMethod);
        }

        private void AddPropertyDefinition(PropertyInfo property, MethodBuilder getMethod, MethodBuilder setMethod)
        {
            PropertyBuilder newProperty = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType,
                Seq.Make(property.GetIndexParameters()).Map<Type>(delegate(ParameterInfo param) { return param.ParameterType; }).ToArray());

            if(getMethod != null)
            {
                newProperty.SetGetMethod(getMethod);
            }

            if(setMethod != null)
            {
                newProperty.SetSetMethod(setMethod);
            }
        }

        private MethodBuilder OverridePropertyMethod(MethodInfo method, int count)
        {
            return method == null ? null : new InterfaceMethodOverride(typeBuilder, pipelineManagerField, targetField, method, count).AddMethod();
        }

        private void AddConstructor()
        {
            Type[] paramTypes = Seq.Collect(typeToIntercept).ToArray();

            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.HasThis,
                paramTypes);

            ctorBuilder.DefineParameter(1, ParameterAttributes.None, "target");
            ILGenerator il = ctorBuilder.GetILGenerator();

            // Call base class constructor

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, ObjectMethods.Constructor);

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, PipelineManagerMethods.Constructor);
            il.Emit(OpCodes.Stfld, pipelineManagerField);

            // Initialize the target field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, targetField);

            il.Emit(OpCodes.Ret);
        }

        private void CreateTypeBuilder()
        {
            TypeAttributes newAttributes = TypeAttributes.Public | TypeAttributes.Class;

            ModuleBuilder moduleBuilder = GetModuleBuilder();
            typeBuilder = moduleBuilder.DefineType(CreateTypeName(), newAttributes);

            DefineGenericArguments();

            pipelineManagerField = InterceptingProxyImplementor.ImplementIInterceptingProxy(typeBuilder);
            targetField = typeBuilder.DefineField("target", typeToIntercept, FieldAttributes.Private);
            typeBuilder.AddInterfaceImplementation(typeToIntercept);
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            string moduleName = Guid.NewGuid().ToString("N");
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            return assemblyBuilder.DefineDynamicModule(moduleName, moduleName + ".dll", true);
#else
            return assemblyBuilder.DefineDynamicModule(moduleName);
#endif
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
                Seq.Make(genericArguments).Map<string>(delegate(Type t) { return t.Name; }).ToArray());

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
