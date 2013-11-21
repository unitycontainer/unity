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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// A simple attribute used to "tag" classes, methods, or properties with a
    /// string that can later be matched via the <see cref="TagAttributeMatchingRule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class TagAttribute : Attribute
    {
        private readonly string tag;

        /// <summary>
        /// Creates a new <see cref="TagAttribute"/> with the given string.
        /// </summary>
        /// <param name="tag">The tag string.</param>
        public TagAttribute(string tag)
        {
            this.tag = tag;
        }

        /// <summary>
        /// The string tag for this attribute.
        /// </summary>
        /// <value>the tag.</value>
        public string Tag
        {
            get { return tag; }
        }
    }
}
