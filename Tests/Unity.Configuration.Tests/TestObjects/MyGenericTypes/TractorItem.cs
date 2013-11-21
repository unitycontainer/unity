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
    class TractorItem : IItem
    {
        public TractorItem(string name, int maxTrailerTons)
        {
            this.ItemName = name;
            this.MaxTrailerWeightTons = maxTrailerTons;
        }

        #region IItem Members

        public string ItemName { get; set; }

        public string ItemType
        {
            get { return "Tractor"; }
        }

        public ItemCategory Category
        {
            get { return ItemCategory.Agricultural; }
        }

        #endregion

        public int MaxTrailerWeightTons { get; set; }
    }
}
