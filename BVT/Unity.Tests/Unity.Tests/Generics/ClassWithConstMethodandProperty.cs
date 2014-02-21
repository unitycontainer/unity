// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public class ClassWithConstMethodandProperty<T>
    {
        private T value;
        public ClassWithConstMethodandProperty()
        { }
        public ClassWithConstMethodandProperty(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public void SetValue(T value)
        {
            this.value = value;
        }
    }
}
