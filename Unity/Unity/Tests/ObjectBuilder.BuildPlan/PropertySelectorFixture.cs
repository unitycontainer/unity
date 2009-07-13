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
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2.Tests.TestDoubles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class PropertySelectorFixture
    {
        [TestMethod]
        public void SelectorReturnsEmptyListWhenObjectHasNoSettableProperties()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(object));

            Assert.AreEqual(0, properties.Count);
        }

        [TestMethod]
        public void SelectorReturnsOnlySettablePropertiesMarkedWithAttribute()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(ClassWithProperties));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual("PropTwo", properties[0].Name);
        }

        [TestMethod]
        public void SelectorIgnoresIndexers()
        {
            List<PropertyInfo> properties = SelectProperties(typeof(ClassWithIndexer));

            Assert.AreEqual(1, properties.Count);
            Assert.AreEqual("Key", properties[0].Name);
        }

        private List<PropertyInfo> SelectProperties(Type t)
        {
            IPropertySelectorPolicy selector = new PropertySelectorPolicy<DependencyAttribute>();
            List<SelectedProperty> properties =  new List<SelectedProperty>(selector.SelectProperties(GetContext(t)));
            return Seq.Make(properties)
                .Map<PropertyInfo>(delegate(SelectedProperty sp) { return sp.Property; })
                .ToList();
        }

        private MockBuilderContext GetContext(Type t)
        {
            MockBuilderContext context = new MockBuilderContext();
            context.BuildKey = t;
            return context;
        }
    }

    class ClassWithProperties
    {
        private string two;
        private string three;

        [Dependency]
        public string PropOne
        {
            get { return null; }
        }

        [Dependency]
        public string PropTwo
        {
            get { return two; }
            set { two = value; }
        }

        public string PropThree
        {
            get { return three; }
            set { three = value; }
        }
    }

    class ClassWithIndexer
    {
        private string key;

        [Dependency]
        public string this[int index]
        {
            get { return null; }
            set { key = index.ToString() + value; }
        }

        [Dependency]
        public string Key
        {
            get { return key; }
            set { key = value; }
        }
    }
}
