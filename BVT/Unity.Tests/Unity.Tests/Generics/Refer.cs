// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public class Refer<TEntity>
    {
        private string str;

        public string Str
        {
            get { return str; }
            set { str = value; }
        }

        public Refer()
        {
            str = "Hello";
        }
    }
}