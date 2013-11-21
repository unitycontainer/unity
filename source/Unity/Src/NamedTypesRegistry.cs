// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Practices.Unity
{
    // A helper class to manage the names that get
    // registered in the container
    class NamedTypesRegistry
    {
        private readonly Dictionary<Type, List<string>> registeredKeys;
        private readonly NamedTypesRegistry parent;

        public NamedTypesRegistry()
            : this(null)
        {
            
        }

        public NamedTypesRegistry(NamedTypesRegistry parent)
        {
            this.parent = parent;
            registeredKeys = new Dictionary<Type, List<string>>();
        }

        public void RegisterType(Type t, string name)
        {
            if(!registeredKeys.ContainsKey(t))
            {
                registeredKeys[t] = new List<string>();
            }

            RemoveMatchingKeys(t, name);
            registeredKeys[t].Add(name);
        }

        public IEnumerable<string> GetKeys(Type t)
        {
            var keys = Enumerable.Empty<string>();

            if(parent != null)
            {
                keys = keys.Concat(parent.GetKeys(t));
            }

            if(registeredKeys.ContainsKey(t))
            {
                keys = keys.Concat(registeredKeys[t]);
            }

            return keys;
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get
            {
                return registeredKeys.Keys;
            }
        }

        public void Clear()
        {
            registeredKeys.Clear();
        }

        // We need to do this the long way - Silverlight doesn't support List<T>.RemoveAll(Predicate)
        private void RemoveMatchingKeys(Type t, string name)
        {
            var uniqueNames = from registeredName in registeredKeys[t]
                              where registeredName != name
                              select registeredName;

            registeredKeys[t] = uniqueNames.ToList();
        }
    }
}
