using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity.Utility;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Class that handles generating the dynamic types used for interception.
    /// </summary>
    public class InterceptingClassGenerator
    {
        private readonly List<MethodInfo> methodsToIntercept;
        private readonly Type typeToIntercept;
        private static readonly AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Unity_ILEmit_DynamicClasses"), AssemblyBuilderAccess.RunAndSave);

        private FieldBuilder pipelineManagerField;
        private TypeBuilder typeBuilder;

        /// <summary>
        /// Create a new <see cref="InterceptingClassGenerator"/> that will generate a
        /// wrapper class for the requested <paramref name="typeToIntercept"/>.
        /// </summary>
        /// <param name="typeToIntercept">Type to generate the wrapper for.</param>
        /// <param name="methodsToIntercept">Sequence of methods on the target type to override.</param>
        public InterceptingClassGenerator(Type typeToIntercept, IEnumerable<MethodImplementationInfo> methodsToIntercept)
        {
            this.typeToIntercept = typeToIntercept;
            this.methodsToIntercept = Seq.Make(methodsToIntercept).Map<MethodInfo>(
                delegate(MethodImplementationInfo method) { return method.ImplementationMethodInfo; }).ToList();
            CreateTypeBuilder();

        }

        /// <summary>
        /// Create the wrapper class for the given type.
        /// </summary>
        /// <returns>Wrapper type.</returns>
        public Type GenerateType()
        {
            int methodNum = 0;
            foreach (MethodInfo method in FilteredMethodsToIntercept())
            {
                new MethodOverride(typeBuilder, pipelineManagerField, method, methodNum++).AddMethod();
            }

            foreach (ConstructorInfo ctor in typeToIntercept.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                AddConstructor(ctor);
            }

            Type result = typeBuilder.CreateType();
#if DEBUG_SAVE_GENERATED_ASSEMBLY
            // assemblyBuilder.Save("Unity_ILEmit_DynamicClasses.dll");
#endif
            return result;
        }

        // Filter out new virtuals that hide other methods
        private IEnumerable<MethodInfo> FilteredMethodsToIntercept()
        {
            Dictionary<string, List<MethodInfo>> methodsByName = new Dictionary<string, List<MethodInfo>>();
            foreach (MethodInfo method in methodsToIntercept)
            {
                if (!methodsByName.ContainsKey(method.Name))
                {
                    methodsByName[method.Name] = new List<MethodInfo>();
                }
                methodsByName[method.Name].Add(method);
            }

            foreach (KeyValuePair<string, List<MethodInfo>> methodList in methodsByName)
            {
                if (methodList.Value.Count == 1)
                {
                    yield return methodList.Value[0];
                }
                else
                {
                    foreach (MethodInfo method in RemoveHiddenOverloads(methodList.Value))
                    {
                        yield return method;
                    }
                }
            }
        }

        private IEnumerable<MethodInfo> RemoveHiddenOverloads(List<MethodInfo> methods)
        {
            // Group the methods by signature
            List<MethodInfo> methodsByParameters = new List<MethodInfo>(methods);
            methodsByParameters.Sort(CompareMethodInfosByParameterLists);
            List<List<MethodInfo>> overloadGroups = new List<List<MethodInfo>>(GroupOverloadedMethods(methodsByParameters));

            foreach(List<MethodInfo> overload in overloadGroups)
            {
                yield return SelectMostDerivedOverload(overload);
            }

        }

        private static IEnumerable<List<MethodInfo>> GroupOverloadedMethods(List<MethodInfo> sortedMethods)
        {
            int index = 0;
            while(index < sortedMethods.Count)
            {
                int overloadStart = index;
                List<MethodInfo> overloads = new List<MethodInfo>();
                overloads.Add(sortedMethods[overloadStart]);
                ++index;
                while(index < sortedMethods.Count && 
                    CompareMethodInfosByParameterLists(sortedMethods[overloadStart], sortedMethods[index]) == 0)
                {
                    overloads.Add(sortedMethods[index++]);
                }

                yield return overloads;
            }
        }

        private MethodInfo SelectMostDerivedOverload(List<MethodInfo> overloads)
        {
            if(overloads.Count == 1)
            {
                return overloads[0];
            }

            int minDepth = int.MaxValue;
            MethodInfo selectedMethod = null;
            foreach(MethodInfo method in overloads)
            {
                int thisDepth = DeclarationDepth(method);
                if(thisDepth < minDepth)
                {
                    minDepth = thisDepth;
                    selectedMethod = method;
                }
            }

            return selectedMethod;
        }

        private int DeclarationDepth(MethodInfo method)
        {
            int depth = 0;
            Type declaringType = typeToIntercept;
            while(declaringType != null && method.DeclaringType != declaringType)
            {
                ++depth;
                declaringType = declaringType.BaseType;
            }
            return depth;
        }

        private static int CompareMethodInfosByParameterLists(MethodInfo left, MethodInfo right)
        {
            return CompareParameterLists(left.GetParameters(), right.GetParameters());    
        }

        private static int CompareParameterLists(ParameterInfo[] left, ParameterInfo[] right)
        {
            if(left.Length != right.Length)
            {
                return left.Length - right.Length;
            }

            for(int i= 0; i < left.Length; ++i)
            {
                int comparison = CompareParameterInfo(left[i], right[i]);
                if(comparison != 0)
                {
                    return comparison;
                }
            }
            return 0;
        }

        private static int CompareParameterInfo(ParameterInfo left, ParameterInfo right)
        {
            if (left.ParameterType == right.ParameterType)
            {
                return 0;
            }
            return left.ParameterType.FullName.CompareTo(right.ParameterType.FullName);
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
            return assemblyBuilder.DefineDynamicModule(moduleName /*, moduleName + ".dll", true */);
#else
            return assemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }
    }
}
