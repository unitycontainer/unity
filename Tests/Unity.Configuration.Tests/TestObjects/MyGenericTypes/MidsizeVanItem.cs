// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    internal class MidsizeVanItem : IItem
    {
        public MidsizeVanItem(string name, int maxCapacity)
        {
            this.ItemName = name;
            this.MaxCapacityCubicFeet = maxCapacity;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Midsize Van"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Van; }
        }

        #endregion

        public int MaxCapacityCubicFeet { get; set; }
    }
}
