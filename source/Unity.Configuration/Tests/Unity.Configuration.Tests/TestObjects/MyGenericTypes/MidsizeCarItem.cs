// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

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
