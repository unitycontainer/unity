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

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
	[TestClass]
	public class BuildKeyMappingPolicyTest
	{
		[TestMethod]
		public void PolicyReturnsNewBuildKey()
		{
			BuildKeyMappingPolicy policy = new BuildKeyMappingPolicy(typeof(string));

			Assert.AreEqual<object>(typeof(string), policy.Map(typeof(object)));
		}
	}
}
