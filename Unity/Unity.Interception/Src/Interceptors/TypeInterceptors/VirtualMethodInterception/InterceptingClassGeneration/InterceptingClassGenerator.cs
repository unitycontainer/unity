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

//#define DEBUG_SAVE_GENERATED_ASSEMBLY

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using Microsoft.Practices.ObjectBuilder2;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Class that handles generating the dynamic types used for interception.
    /// </summary>
    public class InterceptingClassGenerator
    {
        private readonly Type typeToIntercept;

        private static readonly AssemblyBuilder assemblyBuilder;

        private FieldBuilder pipelineManagerField;
        private TypeBuilder typeBuilder;

        [SecurityCritical]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", 
            Justification="Need to use constructor so we can place attribute on it.")]
        static InterceptingClassGenerator()
        {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_DynamicClasses"), AssemblyBuilderAccess.RunAndSave);
        }

        /// <summary>
        /// Create a new <see cref="InterceptingClassGenerator"/> that will generate a
        /// wrapper class for the requested <paramref name="typeToIntercept"/>.
        /// </summary>
        /// <param name="typeToIntercept">Type to generate the wrapper for.</param>
        public InterceptingClassGenerator(Type typeToIntercept)
        {
            this.typeToIntercept = typeToIntercept;
            CreateTypeBuilder();

        }

        /// <summary>
        /// Create the wrapper class for the given type.
        /// </summary>
        /// <returns>Wrapper type.</returns>
        public Type GenerateType()
        {
            AddMethods();
            AddProperties();
            AddConstructors();

            Type result = typeBuilder.CreateType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            assemblyBuilder.Save("Unity_ILEmit_DynamicClasses.dll");
#endif
            return result;
        }

        private void AddMethods()
        {
            int methodNum = 0;
            foreach(MethodInfo method in GetMethodsToIntercept())
            {
                new MethodOverride(typeBuilder, pipelineManagerField, method, methodNum++).AddMethod();
            }
        }

        private IEnumerable<MethodInfo> GetMethodsToIntercept()
        {
            List<MethodInfo> methodsToIntercept = new List<MethodInfo>();
            foreach (MethodInfo method in typeToIntercept.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!method.IsSpecialName && MethodOverride.MethodCanBeIntercepted(method))
                {
                    methodsToIntercept.Add(method);
                }
            }

            MethodSorter sorter = new MethodSorter(typeToIntercept, methodsToIntercept);
            foreach(MethodInfo method in sorter)
            {
                yield return method;
            }
        }

        private void AddProperties()
        {
            // We don't actually add new properties to this class. We just override
            // the get / set methods as available. Inheritance makes sure the properties
            // show up properly on the derived class.

            int propertyCount = 0;
            foreach(PropertyInfo property in typeToIntercept.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                OverridePropertyMethod(property.GetGetMethod(), propertyCount);
                OverridePropertyMethod(property.GetSetMethod(), propertyCount);
                ++propertyCount;
            }
        }

        private void OverridePropertyMethod(MethodInfo method, int count)
        {
            if(method != null && MethodOverride.MethodCanBeIntercepted(method))
            {
                new MethodOverride(typeBuilder, pipelineManagerField, method, count).AddMethod();
            }
        }

        private void AddConstructors()
        {
            foreach (ConstructorInfo ctor in typeToIntercept.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                AddConstructor(ctor);
            }
        }

        private void AddConstructor(ConstructorInfo ctor)
        {
            Type[] paramTypes = Sequence.ToArray(
                Sequence.Map<ParameterInfo, Type>(ctor.GetParameters(),
                    delegate(ParameterInfo item) { return item.ParameterType; }));


            ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(
                ctor.Attributes, ctor.CallingConvention, paramTypes);

            ILGenerator il = ctorBuilder.GetILGenerator();

            // Initialize pipeline field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, PipelineManagerMethods.Constructor);
            il.Emit(OpCodes.Stfld, pipelineManagerField);

            // call base class construtor
            il.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < paramTypes.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i + 1);
            }

            il.Emit(OpCodes.Call, ctor);

            il.Emit(OpCodes.Ret);
        }

        private void CreateTypeBuilder()
        {
            TypeAttributes newAttributes = typeToIntercept.Attributes;
            newAttributes = RemoveTypeNesting(newAttributes);

            Type baseClass = GetGenericType(typeToIntercept);

            ModuleBuilder moduleBuilder = GetModuleBuilder();
            typeBuilder = moduleBuilder.DefineType(
                "DynamicModule.ns.Wrapped_" + typeToIntercept.Name + "_" + Guid.NewGuid().ToString("N"),
                newAttributes,
                baseClass);

            DefineGenericArguments(typeBuilder, baseClass);

            pipelineManagerField = InterceptingProxyImplementor.ImplementIInterceptingProxy(typeBuilder);
        }

        private static Type GetGenericType(Type typeToIntercept)
        {
            if (typeToIntercept.IsGenericType)
            {
                return typeToIntercept.GetGenericTypeDefinition();
            }
            return typeToIntercept;
        }

        private static void DefineGenericArguments(TypeBuilder typeBuilder, Type baseClass)
        {
            if (!baseClass.IsGenericType) return;

            Type[] genericArguments = baseClass.GetGenericArguments();

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

        private static TypeAttributes RemoveTypeNesting(TypeAttributes attributes)
        {
            if ((attributes & TypeAttributes.NestedPublic) != 0)
            {
                attributes &= ~TypeAttributes.NestedPublic;
                attributes |= TypeAttributes.Public;
            }
            return attributes;
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
    }
}
