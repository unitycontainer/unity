// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    internal class CompactCarItem : IItem
    {
        public CompactCarItem(string name, int numberDoors)
        {
            this.ItemName = name;
            this.NumberOfDoors = numberDoors;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Compact Car"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Car; }
        }

        #endregion

        public int NumberOfDoors { get; set; }
    }
}
