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
        private readonly MethodInfo methodToOverride;
        private GenericParameterMapper genericParameterMapper;

        public MethodOverrideParameterMapper(MethodInfo methodToOverride)
        {
            this.methodToOverride = methodToOverride;
        }

        public void SetupParameters(MethodBuilder methodBuilder, GenericParameterMapper parentMapper)
        {
            if (methodToOverride.IsGenericMethod)
            {
                var genericArguments = methodToOverride.GetGenericArguments();
                var names = genericArguments.Select(t => t.Name).ToArray();
                var builders = methodBuilder.DefineGenericParameters(names);
                for (int i = 0; i < genericArguments.Length; ++i)
                {
                    builders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                    var constraintTypes =
                        genericArguments[i]
                            .GetGenericParameterConstraints()
                            .Select(ct => parentMapper.Map(ct))
                            .ToArray();

                    var interfaceConstraints = constraintTypes.Where(t => t.IsInterface).ToArray();
                    Type baseConstraint = constraintTypes.Where(t => !t.IsInterface).FirstOrDefault();
                    if (baseConstraint != null)
                    {
                        builders[i].SetBaseTypeConstraint(baseConstraint);
                    }
                    if (interfaceConstraints.Length > 0)
                    {
                        builders[i].SetInterfaceConstraints(interfaceConstraints);
                    }
                }

                this.genericParameterMapper =
                    new GenericParameterMapper(genericArguments, builders.Cast<Type>().ToArray(), parentMapper);
            }
            else
            {
                this.genericParameterMapper = parentMapper;
            }
        }

        public Type GetParameterType(Type originalParameterType)
        {
            return this.genericParameterMapper.Map(originalParameterType);
        }

        public Type GetElementType(Type originalParameterType)
        {
            return GetParameterType(originalParameterType).GetElementType();
        }

        public Type GetReturnType()
        {
            return GetParameterType(methodToOverride.ReturnType);
        }

        public Type[] GenericMethodParameters
        {
            get { return this.genericParameterMapper.GetGeneratedParameters(); }
        }
    }
}
