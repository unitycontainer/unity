// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Lifetime
{
    public class UnityTestClass
    {
        private string name = "Hello";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}