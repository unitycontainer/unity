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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// An <see cref="IMatchingRule"/> implementation that matches properties
    /// by name. You can match the getter, setter, or both.
    /// </summary>
    public class PropertyMatchingRule : IMatchingRule
    {
        private readonly List<Glob> patterns = new List<Glob>();

        /// <summary>
        /// Construct a new <see cref="PropertyMatchingRule"/> that matches the
        /// getter or setter of the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property. Name comparison is case sensitive. Wildcards are allowed.</param>
        public PropertyMatchingRule(string propertyName)
            : this( propertyName, PropertyMatchingOption.GetOrSet, false)
        {
            
        }

        /// <summary>
        /// Constructs a new <see cref="PropertyMatchingRule"/> that matches the
        /// given method of the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property. Name comparison is case sensitive. Wildcards are allowed.</param>
        /// <param name="option">Match the getter, setter, or both.</param>
        public PropertyMatchingRule(string propertyName, PropertyMatchingOption option)
            : this(propertyName, option, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="PropertyMatchingRule"/> that matches the
        /// given method of the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property to match. Wildcards are allowed.</param>
        /// <param name="option">Match the getter, setter, or both.</param>
        /// <param name="ignoreCase">If false, name comparison is case sensitive. If true, name comparison is case insensitive.</param>
        public PropertyMatchingRule(string propertyName, PropertyMatchingOption option, bool ignoreCase)
            : this( new PropertyMatchingInfo[] { new PropertyMatchingInfo(propertyName, option, ignoreCase)})
        {
        }

        /// <summary>
        /// Constructs a new <see cref="PropertyMatchingRule"/> that matches any of the
        /// given properties.
        /// </summary>
        /// <param name="matches">Collection of <see cref="PropertyMatchingInfo"/> defining which
        /// properties to match.</param>
        public PropertyMatchingRule(IEnumerable<PropertyMatchingInfo> matches)
        {
            foreach(PropertyMatchingInfo match in matches)
            {
                if(match.Option != PropertyMatchingOption.Set)
                {
                    patterns.Add(new Glob("get_" + match.Match, !match.IgnoreCase));
                }

                if(match.Option != PropertyMatchingOption.Get)
                {
                    patterns.Add(new Glob("set_" + match.Match, !match.IgnoreCase));
                }                
            }
        }

        /// <summary>
        /// Checks if the given member matches the rule.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>True if it matches, false if it does not.</returns>
        public bool Matches(MethodBase member)
        {
            return
                member.IsSpecialName &&
                    ( patterns.Exists(
                        delegate(Glob pattern)
                        {
                             return pattern.IsMatch(member.Name);
                        }) 
                    );
        }
    }

    /// <summary>
    /// Specifies which methods of a property should be matches by
    /// the <see cref="PropertyMatchingRule"/>.
    /// </summary>
    public enum PropertyMatchingOption
    {
        /// <summary>
        /// Match the property getter method.
        /// </summary>
        Get,
        /// <summary>
        /// Match the property setter method.
        /// </summary>
        Set,
        /// <summary>
        /// Match either the getter or setter method.
        /// </summary>
        GetOrSet
    }

    /// <summary>
    /// Information about a property match.
    /// </summary>
    public class PropertyMatchingInfo : MatchingInfo
    {
        private PropertyMatchingOption option;

        /// <summary>
        /// Construct a new <see cref="PropertyMatchingInfo"/> that matches the get or set methods
        /// of the given property name, and does a case-sensitive comparison.
        /// </summary>
        /// <param name="match">Property name to match.</param>
        public PropertyMatchingInfo(string match) : this( match, PropertyMatchingOption.GetOrSet, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="PropertyMatchingInfo"/> that matches the given methods of
        /// the given property name, doing a case-sensitive comparison.
        /// </summary>
        /// <param name="match">Property name to match.</param>
        /// <param name="option"><see cref="PropertyMatchingOption"/> specifying which methods of the property to match.</param>
        public PropertyMatchingInfo(string match, PropertyMatchingOption option) : this(match, option, false)
        {
        }

        /// <summary>
        /// Construct a new <see cref="PropertyMatchingInfo"/> that matches the given methods of
        /// the given property name.
        /// </summary>
        /// <param name="match">Property name to match.</param>
        /// <param name="option"><see cref="PropertyMatchingOption"/> specifying which methods of the property to match.</param>
        /// <param name="ignoreCase">If false, name comparison is case sensitive. If true, name comparison is case insensitive.</param>
        public PropertyMatchingInfo(string match, PropertyMatchingOption option, bool ignoreCase)
            : base( match, ignoreCase)
        {
            this.option = option;
        }

        /// <summary>
        /// The <see cref="PropertyMatchingOption"/> to use when doing name comparisons on this property.
        /// </summary>
        /// <value>Specifies which methods of the property to match.</value>
        public PropertyMatchingOption Option
        {
            get { return option; }
            set { option = value; }
        }
    }
}
