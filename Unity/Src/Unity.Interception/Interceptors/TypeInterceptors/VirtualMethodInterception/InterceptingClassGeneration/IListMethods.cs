using System.Collections;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    internal static class IListMethods
    {
        internal static MethodInfo GetItem
        {
            get { return typeof(IList).GetProperty("Item").GetGetMethod(); }
        }
    }
}