// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Practices.Unity
{
    internal class RegisteredNamesPolicy : IRegisteredNamesPolicy
    {
        private NamedTypesRegistry registry;

        public RegisteredNamesPolicy(NamedTypesRegistry registry)
        {
            this.registry = registry;
        }

        public IEnumerable<string> GetRegisteredNames(Type type)
        {
            return this.registry.GetKeys(type).Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
