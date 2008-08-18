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
        private Dictionary<Type, List<string>> registeredKeys;
        private NamedTypesRegistry parent;

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

                registeredKeys[t].RemoveAll(delegate(string s) { return name == s; });

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
    }
}
