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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// This class handles parameter type mapping. When we generate
    /// a generic method, we need to make sure our parameter type
    /// objects line up with the generic parameters on the generated
    /// method, not on the one we're overriding. 
    /// </summary>
    internal class MethodOverrideParameterMapper
    {
        private readonly List<KeyValuePair<Type, Type>> genericMethodParameters = new List<KeyValuePair<Type, Type>>();
        private readonly MethodInfo methodToOverride;

        public MethodOverrideParameterMapper(MethodInfo methodToOverride)
        {
            this.methodToOverride = methodToOverride;    
        }

        public void SetupParameters(MethodBuilder methodBuilder)
        {
            if (methodToOverride.IsGenericMethod)
            {
                Type[] genericArguments = methodToOverride.GetGenericArguments();
                string[] names = genericArguments
                    .Select(t => t.Name)
                    .ToArray();
                GenericTypeParameterBuilder[] builders = methodBuilder.DefineGenericParameters(names);
                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    builders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                    var constraintTypes = genericArguments[i].GetGenericParameterConstraints();

                    builders[i].SetInterfaceConstraints(constraintTypes.Where(t => t.IsInterface).ToArray());
                    foreach (Type type in constraintTypes.Where(t => !t.IsInterface))
                    {
                        builders[i].SetBaseTypeConstraint(type);
                    }
                }
                for(int i = 0; i < genericArguments.Length; ++i)
                {
                    genericMethodParameters.Add(new KeyValuePair<Type, Type>(genericArguments[i], builders[i].UnderlyingSystemType));
                }
            }
        }

        public Type GetParameterType(Type originalParameterType)
        {
            var mappedParameter = (from param in genericMethodParameters
                where param.Key == originalParameterType
                select param.Value).FirstOrDefault();

            return mappedParameter ?? originalParameterType;
        }

        public Type GetElementType(Type originalParameterType)
        {
            return GetParameterType(originalParameterType).GetElementType();
        }

        public Type[] MappedGenericParameters
        {
            get { return genericMethodParameters.Select(kvp => kvp.Value).ToArray(); }
        }

        public Type GetReturnType()
        {
            return GetParameterType(methodToOverride.ReturnType);
        }
    }
}
