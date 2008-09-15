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

namespace Microsoft.Practices.Unity.InterceptionExtension
{
    /// <summary>
    /// Class used for storing information about a single name/ignoreCase
    /// pair. This class is also used as a base class for other classes that
    /// need this pair plus some other properties.
    /// </summary>
    public class MatchingInfo
    {
        private string match;
        private bool ignoreCase;

        /// <summary>
        /// Constructs an empty <see cref="MatchingInfo"/> object with empty
        /// string and ignoreCase = false.
        /// </summary>
        public MatchingInfo()
            : this(string.Empty, false)
        {
        }

        /// <summary>
        /// Constructs a <see cref="MatchingInfo"/> object that matches the given
        /// string. IgnoreCase is false.
        /// </summary>
        /// <param name="nameToMatch">The name to match.</param>
        public MatchingInfo(string nameToMatch)
            : this(nameToMatch, false)
        {
        }

        /// <summary>
        /// Constructs a <see cref="MatchingInfo"/> object that matches the
        /// given string, setting the ignoreCase flag to the given value.
        /// </summary>
        /// <param name="nameToMatch">The name to match.</param>
        /// <param name="ignoreCase">true to do case insensitive comparison, false to do case sensitive.</param>
        public MatchingInfo(string nameToMatch, bool ignoreCase)
        {
            this.match = nameToMatch;
            this.ignoreCase = ignoreCase;
        }

        /// <summary>
        /// Gets or sets the name to match.
        /// </summary>
        /// <value>The name to match.</value>
        public string Match
        {
            get { return match; }
            set { match = value; }
        }

        /// <summary>
        /// Gets or sets whether to do case sensitive comparisons of Match.
        /// </summary>
        /// <value>If false, case sensitive comparison. If true, case insensitive comparisons.</value>
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }
    }
}