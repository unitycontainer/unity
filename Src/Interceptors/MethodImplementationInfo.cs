// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Globalization;
using System.Reflection;

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A dumb data holder that returns the MethodInfo for both an
    /// interface method and the method that implements that interface
    /// method.
    /// </summary>
    public class MethodImplementationInfo
    {
        private readonly MethodInfo interfaceMethodInfo;
        private readonly MethodInfo implementationMethodInfo;

        /// <summary>
        /// Construct a new <see cref="MethodImplementationInfo"/> which holds
        /// the given <see cref="MethodInfo"/> objects.
        /// </summary>
        /// <param name="interfaceMethodInfo">MethodInfo for the interface method (may be null if no interface).</param>
        /// <param name="implementationMethodInfo">MethodInfo for implementing method.</param>
        public MethodImplementationInfo(MethodInfo interfaceMethodInfo, MethodInfo implementationMethodInfo)
        {
            this.interfaceMethodInfo = interfaceMethodInfo;
            this.implementationMethodInfo = implementationMethodInfo;
        }

        /// <summary>
        /// The interface method MethodInfo.
        /// </summary>
        public MethodInfo InterfaceMethodInfo
        {
            get { return interfaceMethodInfo; }
        }

        /// <summary>
        /// The implementing method MethodInfo.
        /// </summary>
        public MethodInfo ImplementationMethodInfo
        {
            get { return implementationMethodInfo; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj" /> parameter is null.</exception>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            MethodImplementationInfo other = obj as MethodImplementationInfo;
            if (obj == null || other == null)
            {
                return false;
            }

            return this == other;
        }

        /// <summary>
        ///                     Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        ///                     A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hash1 = interfaceMethodInfo != null ? interfaceMethodInfo.GetHashCode() : 0;
            int hash2 = implementationMethodInfo != null ? implementationMethodInfo.GetHashCode() : 0;
            return hash1 * 23 ^ hash2 * 7;
        }

        /// <summary>
        /// Standard equals operator
        /// </summary>
        public static bool operator ==(MethodImplementationInfo left, MethodImplementationInfo right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return left.interfaceMethodInfo == right.interfaceMethodInfo &&
                left.implementationMethodInfo == right.implementationMethodInfo;
        }

        /// <summary>
        /// standard not equal operator.
        /// </summary>
        public static bool operator !=(MethodImplementationInfo left, MethodImplementationInfo right)
        {
            return !(left == right);
        }

        /// <summary>
        ///                     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///                     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            if (interfaceMethodInfo == null)
            {
                return string.Format(CultureInfo.CurrentCulture,
                    "No interface, implementation {0}.{1}",
                    implementationMethodInfo.DeclaringType.Name, implementationMethodInfo.Name);
            }
            return string.Format(CultureInfo.CurrentCulture,
                "Interface {0}.{1}, implementation {2}.{3}",
                interfaceMethodInfo.DeclaringType.Name, interfaceMethodInfo.Name,
                implementationMethodInfo.DeclaringType.Name, implementationMethodInfo.Name);
        }
    }
}
