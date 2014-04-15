using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Practices.Unity.TestSupport
{
    internal static class TypeReflectionExtensions
    {
        public static ConstructorInfo GetMatchingConstructor(this Type type, Type[] constructorParamTypes)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(constructorParamTypes))
                .FirstOrDefault();
        }
    }
}
