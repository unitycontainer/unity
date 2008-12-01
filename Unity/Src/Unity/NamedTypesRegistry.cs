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
            if(!string.IsNullOrEmpty(name))
            {
                if(!registeredKeys.ContainsKey(t))
                {
                    registeredKeys[t] = new List<string>();
                }

                RemoveMatchingKeys(t, name);
                registeredKeys[t].Add(name);
            }
        }

        public IEnumerable<string> GetKeys(Type t)
        {
            if(parent != null)
            {
                foreach(string name in parent.GetKeys(t))
                {
                    yield return name;
                }
            }

            if(!registeredKeys.ContainsKey(t))
            {
                yield break;
            }
            foreach(string name in registeredKeys[t])
            {
                yield return name;
            }
        }

        public void Clear()
        {
            registeredKeys.Clear();
        }

        // We need to do this the long way - Silverlight doesn't support List<T>.RemoveAll(Predicate)
        private void RemoveMatchingKeys(Type t, string name)
        {
            List<int> indexes = new List<int>();
            int i = 0;
            foreach(string s in registeredKeys[t])
            {
                if(name == s)
                {
                    indexes.Add(i);
                }
                ++i;
            }

            indexes.Reverse();
            foreach(int index in indexes)
            {
                registeredKeys[t].RemoveAt(index);
            }
        }
    }
}
