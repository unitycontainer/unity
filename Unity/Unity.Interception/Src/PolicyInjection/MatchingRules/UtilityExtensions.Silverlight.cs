using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A set of convenience extension methods to replicate methods missing in Silverlight
    /// that the desktop CLR version is using.
    /// </summary>
    internal static class UtilityExtensions
    {
        internal static bool Exists<TItem>(this IEnumerable<TItem> sequence, Func<TItem, bool> predicate)
        {
            return sequence.Any(predicate);
        }

        internal static int FindIndex<TItem>(this IEnumerable<TItem> sequence, Func<TItem, bool> predicate)
        {
            var indexedItems = sequence.Select((item, index) => Tuple.Create(index, item))
                .Where(pair => predicate(pair.Item2))
                .Select(pair => pair.Item1);

            foreach (var item in indexedItems)
            {
                return item;
            }

            return -1;
        }

        internal static bool IsReturn(this ParameterInfo parameterInfo)
        {
            return ((parameterInfo.Attributes & ParameterAttributes.Retval) != ParameterAttributes.None);
        }

        internal static bool IsInvariantCulture(this CultureInfo cultureInfo)
        {
            return cultureInfo.Name == CultureInfo.InvariantCulture.Name;
        }

        internal static bool IsSameAs(this CultureInfo cultureInfo, CultureInfo otherCultureInfo)
        {
            return (cultureInfo == null && otherCultureInfo == null) ||
                (cultureInfo != null && otherCultureInfo != null &&
                    cultureInfo.Name == otherCultureInfo.Name);
        }
    }
}
