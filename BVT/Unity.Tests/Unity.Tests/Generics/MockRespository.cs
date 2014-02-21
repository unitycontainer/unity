// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using Microsoft.Practices.Unity;

namespace Unity.Tests.Generics
{
    public class MockRespository<TEntity> : IRepository<TEntity>
    {
        private Refer<TEntity> obj;

        [Dependency]
        public Refer<TEntity> Add
        {
            get { return obj; }
            set { obj = value; }
        }

        [InjectionConstructor]
        public MockRespository(Refer<TEntity> obj)
        { }
    }
}