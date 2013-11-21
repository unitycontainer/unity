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

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// Object containing the result of attempting to match a PEG rule.
    /// This object is the return type for all parsing methods.
    /// </summary>
    internal class ParseResult
    {
        public ParseResult(bool matched)
        {
            Matched = matched;
            MatchedString = string.Empty;
            ResultData = null;
        }

        public ParseResult(string matchedCharacters)
            : this(matchedCharacters, null)
        {
            Matched = true;
            MatchedString = matchedCharacters ?? string.Empty;
        }

        public ParseResult(string matchedCharacters, object resultData)
        {
            Matched = true;
            MatchedString = matchedCharacters ?? string.Empty;
            ResultData = resultData;
        }

        /// <summary>
        /// Did the rule match?
        /// </summary>
        public bool Matched { get; private set; }

        /// <summary>
        /// The characters that were matched (if any)
        /// </summary>
        public string MatchedString { get; private set; }

        /// <summary>
        /// Any extra information provided by the parsing expression
        /// (only set if the parse matched). The nature
        /// of the data varies depending on the parsing expression.
        /// </summary>
        public object ResultData { get; private set; }
    }
}
