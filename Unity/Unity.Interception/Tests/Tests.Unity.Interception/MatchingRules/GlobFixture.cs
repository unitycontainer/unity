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

namespace Microsoft.Practices.Unity.InterceptionExtension.Tests.MatchingRules
{
    /// <summary>
    /// Test figure for the Glob class, which implements matching using
    /// file system style patterns for simple pattern matches rather than
    /// full-bore regexes.
    /// </summary>
    [TestClass]
    public class GlobFixture
    {
        [TestMethod]
        public void ShouldMatchExactlyWhenNoWildcards()
        {
            Glob glob = new Glob("MyClass");
            Assert.IsTrue(glob.IsMatch("MyClass"));
            Assert.IsFalse(glob.IsMatch("MyClass2"));
            Assert.IsFalse(glob.IsMatch("ReallyMyClass"));
        }

        [TestMethod]
        public void ShouldMatchWithTrailingWildcard()
        {
            Glob glob = new Glob("MyClass*");
            Assert.IsTrue(glob.IsMatch("MyClass"));
            Assert.IsTrue(glob.IsMatch("MyClassAndMore2"));
            Assert.IsFalse(glob.IsMatch("ReallyMyClass"));
        }

        [TestMethod]
        public void ShouldMatchWithLeadingWildcard()
        {
            Glob glob = new Glob("*Class");
            Assert.IsTrue(glob.IsMatch("MyClass"));
            Assert.IsTrue(glob.IsMatch("My.other.Class"));
            Assert.IsFalse(glob.IsMatch("MyClassAndMore2"));
        }

        [TestMethod]
        public void ShouldBeCaseSensitiveByDefault()
        {
            Glob glob = new Glob("MyClass");
            Assert.IsFalse(glob.IsMatch("myclass"));
        }

        [TestMethod]
        public void ShouldMatchWithCaseInsensitiveFlag()
        {
            Glob glob = new Glob("MyClass", false);

            Assert.IsTrue(glob.IsMatch("myclass"));
        }

        [TestMethod]
        public void DotsShouldBeLiterals()
        {
            Glob glob = new Glob("*.cs");

            Assert.IsTrue(glob.IsMatch("foo.cs"));
            Assert.IsFalse(glob.IsMatch("foobarcs"));
        }

        [TestMethod]
        public void BracketsShouldMatchSingleCharacters()
        {
            Glob glob = new Glob("Test[0-9][0-9]");
            Assert.IsTrue(glob.IsMatch("Test01"));
            Assert.IsTrue(glob.IsMatch("Test54"));
            Assert.IsFalse(glob.IsMatch("Test200"));
        }

        [TestMethod]
        public void QuestionMarksShouldMatchSingleCharacters()
        {
            Glob glob = new Glob("foo??bar");

            Assert.IsTrue(glob.IsMatch("foo00bar"));
            Assert.IsTrue(glob.IsMatch("fooWEbar"));
            Assert.IsFalse(glob.IsMatch("fooTooManybar"));
            Assert.IsTrue(glob.IsMatch("foo??bar"));
        }

        [TestMethod]
        public void DollarSignsAndCaretsAreLiterals()
        {
            Glob glob = new Glob("abc$def^*");
            Assert.IsTrue(glob.IsMatch("abc$def^"));
            Assert.IsTrue(glob.IsMatch("abc$def^Stuff"));
            Assert.IsFalse(glob.IsMatch("abc$"));
        }

        [TestMethod]
        public void ShouldMatchNothingWithEmptyPattern()
        {
            Glob glob = new Glob(string.Empty);
            Assert.IsFalse(glob.IsMatch("a"));
        }
    }
}
