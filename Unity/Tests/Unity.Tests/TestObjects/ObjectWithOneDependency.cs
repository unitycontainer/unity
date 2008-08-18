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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
	internal class ObjectWithOneDependency
	{
		private object inner;

		public ObjectWithOneDependency(object inner)
		{
			this.inner = inner;
		}

		public object InnerObject
		{
			get { return inner; }
		}

		public void Validate()
		{
			Assert.IsNotNull(inner);
		}
	}
}
