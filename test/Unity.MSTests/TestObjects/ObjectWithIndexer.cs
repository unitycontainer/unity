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

using Unity;

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
    public class ObjectWithIndexer
	{
		[Dependency]
		public object this[int index]
		{
			get { return null; }
			set { }
		}

		public bool Validate()
		{
			return true;
		}
	}
}
