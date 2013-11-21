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
    class CompactCarItem : IItem
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
