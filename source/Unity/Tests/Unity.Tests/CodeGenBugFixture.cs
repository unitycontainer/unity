// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif __IOS__
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestInitializeAttribute = NUnit.Framework.SetUpAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Unity.Tests
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
            if (parameter == null)
            {
                throw new ArgumentNullException("Static constructor was not called");
            }
        }
    }
}
