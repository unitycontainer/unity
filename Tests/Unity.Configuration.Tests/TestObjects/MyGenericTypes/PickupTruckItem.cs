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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    class PickupTruckItem : IItem
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
