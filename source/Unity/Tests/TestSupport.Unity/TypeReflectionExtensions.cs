// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Unity.TestSupport
{
    public static class TypeReflectionExtensions
    {
        public static ConstructorInfo GetMatchingConstructor(this Type type, Type[] constructorParamTypes)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(constructorParamTypes))
                .FirstOrDefault();
        }
    }
}
