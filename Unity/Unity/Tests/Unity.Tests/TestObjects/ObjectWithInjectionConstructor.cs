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

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    public class ObjectWithInjectionConstructor
    {
        private object constructorDependency;


        public ObjectWithInjectionConstructor(object constructorDependency)
        {
            this.constructorDependency = constructorDependency;
        }

        [InjectionConstructor]
        public ObjectWithInjectionConstructor(string s)
        {
            constructorDependency = s;
        }

        public object ConstructorDependency
        {
            get { return constructorDependency; }
        }
    }
}
