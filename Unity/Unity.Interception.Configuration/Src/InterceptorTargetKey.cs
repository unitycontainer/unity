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

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration
{
    internal struct InterceptorTargetKey
    {
        public InterceptorTargetKey(string name, string typeName, bool isDefault)
        {
            this.name = name;
            this.typeName = typeName;
            this.isDefault = isDefault;
        }

        private string name;
        private string typeName;
        private bool isDefault;

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(InterceptorTargetKey))
            {
                return this == (InterceptorTargetKey)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (SafeGetHashCode(name))
                    ^ (SafeGetHashCode(typeName))
                    ^ (isDefault ? 1 : 0);
        }

        public static bool operator ==(InterceptorTargetKey left, InterceptorTargetKey right)
        {
            return left.typeName == right.typeName
                && left.name == right.name
                && left.isDefault == right.isDefault;
        }

        public static bool operator !=(InterceptorTargetKey left, InterceptorTargetKey right)
        {
            return !(left == right);
        }

        private static int SafeGetHashCode(object obj)
        {
            return obj != null ? obj.GetHashCode() : 0;
        }
    }
}
