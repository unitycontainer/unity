// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.ObjectBuilder2.Tests.TestObjects
{
    internal class ObjectWithMultipleConstructors
    {
        public ObjectWithMultipleConstructors()
        {
        }

        public ObjectWithMultipleConstructors(int first, string second)
        {
        }

        public ObjectWithMultipleConstructors(int first)
        {
        }
    }
}