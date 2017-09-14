// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Configuration.Tests.TestObjects.MyGenericTypes
{
    internal class TractorItem : IItem
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
