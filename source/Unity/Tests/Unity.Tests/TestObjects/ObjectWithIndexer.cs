// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    public class ObjectWithIndexer
    {
        [Dependency]
        public object this[int index]
        {
            get { return null; }
            set { }
        }

        public bool Validate()
        {
            return true;
        }
    }
}
