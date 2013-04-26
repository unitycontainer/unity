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

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests.TestObjects
{
	// A class that contains another one which has another
	// constructor dependency. Used to validate recursive
	// buildup of constructor dependencies.
    public class ObjectWithTwoConstructorDependencies
	{
		private ObjectWithOneDependency oneDep;

		public ObjectWithTwoConstructorDependencies(ObjectWithOneDependency oneDep)
		{
			this.oneDep = oneDep;
		}

	    public ObjectWithOneDependency OneDep
	    {
	        get { return oneDep; }
	    }

	    public void Validate()
		{
			Assert.IsNotNull(oneDep);
			oneDep.Validate();
		}
	}
}
