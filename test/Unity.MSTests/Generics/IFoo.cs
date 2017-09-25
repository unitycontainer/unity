// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
namespace Unity.Tests.Generics
{
    public interface IFoo<TEntity>
    {
        TEntity Value { get; }
    }

    public interface IFoo
    {
    }
}