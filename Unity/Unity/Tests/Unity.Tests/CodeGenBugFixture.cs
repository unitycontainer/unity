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
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Microsoft.Practices.Unity.Tests
{
    /// <summary>
    /// Test for dynamic method creation and the CLR bug. This test will only
    /// fail if run in a release build!
    /// </summary>
    [TestClass]
    public class CodeGenBugFixture
    {
        [TestMethod]
        public void ResolvedTypeHasStaticConstructorCalled()
        {
            IUnityContainer container = new UnityContainer();

            CodeGenBug result = container.Resolve<CodeGenBug>();
        }
    }

    public class CodeGenBug
    {
        public static readonly object TheStaticObject;

        static CodeGenBug()
        {
            TheStaticObject = new object();
        }

        [InjectionConstructor]
        public CodeGenBug()
            : this(-12, TheStaticObject)
        {
        }

        public CodeGenBug(int i, object parameter)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException("Static constructor was not called");
            }
        }
    }
}
