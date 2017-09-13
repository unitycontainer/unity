// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    internal class PickupTruckItem : IItem
    {
        public PickupTruckItem(string name, int maxLoad)
        {
            this.ItemName = name;
            this.MaxLoadPounds = maxLoad;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Pickup Truck"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Truck; }
        }

        #endregion

        public int MaxLoadPounds { get; set; }
    }
}
