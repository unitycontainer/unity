// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    internal class LargeTruckItem : IItem
    {
        public LargeTruckItem(string name, int numberWheels)
        {
            this.ItemName = name;
            this.NumberOfWheels = numberWheels;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Large Truck"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Truck; }
        }

        #endregion

        public int NumberOfWheels { get; set; }
    }
}
