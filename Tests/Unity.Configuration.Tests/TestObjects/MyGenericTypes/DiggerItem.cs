// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Practices.Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    class DiggerItem : IItem
    {

        public DiggerItem(string name, int bucketWidth)
        {
            this.ItemName = name;
            this.BucketWidthInches = bucketWidth;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Digger"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Construction; }
        }

        #endregion

        public int BucketWidthInches { get; set; }
    }
}
