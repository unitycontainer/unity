// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Unity.InterceptionExtension
{
    internal struct GeneratedTypeKey
    {
        private readonly Type baseType;
        private readonly Type[] additionalInterfaces;

        public GeneratedTypeKey(Type baseType, Type[] additionalInterfaces)
        {
            this.baseType = baseType;
            this.additionalInterfaces = additionalInterfaces;
        }

        internal class GeneratedTypeKeyComparer : IEqualityComparer<GeneratedTypeKey>
        {
            public bool Equals(GeneratedTypeKey x, GeneratedTypeKey y)
            {
                if (!(x.baseType.Equals(y.baseType) && x.additionalInterfaces.Length == y.additionalInterfaces.Length))
                {
                    return false;
                }
                for (int i = 0; i < x.additionalInterfaces.Length; i++)
                {
                    if (!x.additionalInterfaces[i].Equals(y.additionalInterfaces[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(GeneratedTypeKey obj)
            {
                return obj.baseType.GetHashCode();
            }
        }
    }
}
