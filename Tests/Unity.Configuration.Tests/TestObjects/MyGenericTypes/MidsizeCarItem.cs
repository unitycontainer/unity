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
    class MidsizeCarItem : IItem
    {
        public MidsizeCarItem(string name, int numberCupHolders)
        {
            this.ItemName = name;
            this.NumberOfCupHolders = numberCupHolders;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Midsize Car"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Car; }
        }

        #endregion

        public int NumberOfCupHolders { get; set; }
    }
}
