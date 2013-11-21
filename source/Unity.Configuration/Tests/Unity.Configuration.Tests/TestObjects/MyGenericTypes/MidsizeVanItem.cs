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
    class MidsizeVanItem : IItem
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
