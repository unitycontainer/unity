// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity
{
    /// <summary>
    /// This attribute is used to indicate which constructor to choose when
    /// the container attempts to build a type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class InjectionConstructorAttribute : Attribute
    {
    }
}
