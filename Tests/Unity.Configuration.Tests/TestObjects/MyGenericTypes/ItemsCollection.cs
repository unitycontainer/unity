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

using System;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    public class ItemsCollection<T>
    {
        public IGenericService<T> Printer;
        public string CollectionName;

        public ItemsCollection(String name, IGenericService<T> printService)
        {
            CollectionName = name;
            Printer = printService;
        }

        public ItemsCollection(String name, IGenericService<T> printService, T[] items)
            : this(name, printService)
        {
            this.Items = items;
        }

        public ItemsCollection(String name, IGenericService<T> printService, T[][] itemsArray)
            : this(name, printService, itemsArray.Length > 0 ? itemsArray[0] : null)
        { }

        public T[] Items { get; set; }

    }
}
