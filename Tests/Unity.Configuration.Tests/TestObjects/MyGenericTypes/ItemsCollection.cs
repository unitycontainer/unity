// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;

namespace Unity.Configuration.Tests.TestObjects.MyGenericTypes
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
