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

namespace Microsoft.Practices.Unity.Configuration.ConfigurationHelpers
{
    /// <summary>
    /// A simple implementing of the rules for a Parsing Expression Grammar
    /// parsing algorithm. This supplies basic methods to do the primitives
    /// of the PEG, and combinators to create larger rules.
    /// </summary>
    internal class ParseBuilder
    {
        private static readonly ParseResult matchFailed = new ParseResult(false);

        /// <summary>
        /// The PEG "dot" operator that matches and consumes one character.
        /// </summary>
        /// <param name="input">Input to the parser.</param>
        /// <returns>The parse result.</returns>
        public static ParseResult Any(InputStream input)
        {
            if (input.AtEnd)
            {
                return matchFailed;
            }

            var result = new ParseResult(input.CurrentChar.ToString());

            input.Consume(1);
            return result;
        }

        /// <summary>
        /// Parse function generator that returns a method to match a single,
        /// specific character.
        /// </summary>
        /// <param name="charToMatch">Character to match.</param>
        /// <returns>The generated parsing function.</returns>
        public static Func<InputStream, ParseResult> Match(char charToMatch)
        {
            return input =>
                {
                    if (!input.AtEnd && input.CurrentChar == charToMatch)
                    {
                        return MatchAndConsumeCurrentChar(input);
                    }
                    return matchFailed;
                };
        }

        public static Func<InputStream, ParseResult> Match(string s)
        {
            return input =>
                {
                    int bookmark = input.CurrentPosition;
                    foreach (char ch in s)
                    {
                        if (!input.AtEnd && input.CurrentChar == ch)
                        {
                            input.Consume(1);
                        }
                        else
                        {
                            input.BackupTo(bookmark);
                            return matchFailed;
                        }
                    }
                    return new ParseResult(s);
                };
        }

        /// <summary>
        /// Parse function generator that checks if the current character matches
        /// the predicate supplied.
        /// </summary>
        /// <param name="predicate">Predicate used to determine if the character is in
        /// the given range.</param>
        /// <returns>The generated parsing function.</returns>
        public static Func<InputStream, ParseResult> Match(Func<char, bool> predicate)
        {
            return input =>
                {
                    if (!input.AtEnd && predicate(input.CurrentChar))
                    {
                        return MatchAndConsumeCurrentChar(input);
                    }
                    return matchFailed;
                };
        }

        /// <summary>
        /// The "*" operator - match zero or more of the inner parse expressions.
        /// </summary>
        /// <param name="inner">Parse method to repeat matching.</param>
        /// <returns>The generated parsing function.</returns>
        public static Func<InputStream, ParseResult> ZeroOrMore(Func<InputStream, ParseResult> inner)
        {
            return input =>
                {
                    var results = new List<ParseResult>();
                    ParseResult result = inner(input);
                    if (!result.Matched)
                    {
                        return new ParseResult(true);
                    }

                    results.Add(result);
                    string matchedString = result.MatchedString;
                    result = inner(input);
                    while (result.Matched)
                    {
                        matchedString += result.MatchedString;
                        results.Add(result);
                        result = inner(input);
                    }
                    return new ParseResult(matchedString, results);
                };
        }

        public static Func<InputStream, ParseResult> ZeroOrOne(Func<InputStream, ParseResult> expression)
        {
            return input =>
                {
                    var result = expression(input);
                    if (result.Matched)
                    {
                        return result;
                    }
                    return new ParseResult(true);
                };
        }

        public static Func<InputStream, ParseResult> OneOrMore(Func<InputStream, ParseResult> expression)
        {
            return input =>
                {
                    int bookmark = input.CurrentPosition;
                    var results = new List<ParseResult>();
                    ParseResult result = expression(input);
                    if (!result.Matched)
                    {
                        input.BackupTo(bookmark);
                        return matchFailed;
                    }
                    string matchedString = "";
                    while (result.Matched)
                    {
                        results.Add(result);
                        matchedString += result.MatchedString;
                        result = expression(input);
                    }
                    return new ParseResult(matchedString, results);
                };
        }

        /// <summary>
        /// Parsing combinator that matches all of the given expressions in
        /// order, or matches none of them.
        /// </summary>
        /// <param name="expressions">Expressions that form the sequence to match.</param>
        /// <returns>The combined sequence.</returns>
        public static Func<InputStream, ParseResult> Sequence(params Func<InputStream, ParseResult>[] expressions)
        {
            return input =>
                {
                    int bookmark = input.CurrentPosition;
                    var results = new List<ParseResult>(expressions.Length);
                    var matchedString = "";

                    foreach (var expression in expressions)
                    {
                        var result = expression(input);
                        if (!result.Matched)
                        {
                            input.BackupTo(bookmark);
                            return matchFailed;
                        }

                        results.Add(result);
                        matchedString += result.MatchedString;
                    }

                    return new ParseResult(matchedString, results);
                };
        }

        /// <summary>
        /// Parsing combinator that implements the PEG prioritized choice operator. Basically,
        /// try each of the expressions in order, and match if any of them match, stopping on the
        /// first match.
        /// </summary>
        /// <param name="expressions">Expressions that form the set of alternatives.</param>
        /// <returns>The combined parsing method.</returns>
        public static Func<InputStream, ParseResult> FirstOf(params Func<InputStream, ParseResult>[] expressions)
        {
            return input =>
                {
                    foreach (var expression in expressions)
                    {
                        ParseResult result = expression(input);
                        if (result.Matched)
                        {
                            return result;
                        }
                    }
                    return matchFailed;
                };
        }

        /// <summary>
        /// Parsing combinator implementing the "not" predicate. This wraps
        /// the given <paramref name="expression"/> parsing method with a check
        /// to see if it matched. If it matched, then the Not fails, and vice
        /// versa. The result consumes no input.
        /// </summary>
        /// <param name="expression">The parse method to wrap.</param>
        /// <returns>The generated parsing function.</returns>
        public static Func<InputStream, ParseResult> Not(Func<InputStream, ParseResult> expression)
        {
            return input =>
                {
                    int bookmark = input.CurrentPosition;
                    ParseResult innerResult = expression(input);
                    input.BackupTo(bookmark);

                    return new ParseResult(!innerResult.Matched);
                };
        }

        /// <summary>
        /// Parsing expression that matches End of input.
        /// </summary>
        /// <param name="input">Parser input.</param>
        /// <returns>Parse result</returns>
        public static ParseResult EOF(InputStream input)
        {
            return new ParseResult(input.AtEnd);
        }

        /// <summary>
        /// Combinator that executes an action if the given expression matched.
        /// </summary>
        /// <param name="expression">Parsing expression to match.</param>
        /// <param name="onMatched">Action to execute if <paramref name="expression"/>
        /// matched. Input is the matched text from <paramref name="expression"/>.</param>
        /// <returns>The result of <paramref name="expression"/>.</returns>
        public static Func<InputStream, ParseResult> WithAction(Func<InputStream, ParseResult> expression, Action<ParseResult> onMatched)
        {
            return input =>
                {
                    ParseResult result = expression(input);
                    if (result.Matched)
                    {
                        onMatched(result);
                    }
                    return result;
                };
        }

        /// <summary>
        /// Combinator that executes an action if the given expression matched.
        /// </summary>
        /// <param name="expression">parsing expression to match.</param>
        /// <param name="onMatched">Method to execute if a match happens. This method returns
        /// the <see cref="ParseResult"/> that will be returned from the combined expression.</param>
        /// <returns>The result of <paramref name="onMatched"/> if expression matched, else
        /// whatever <paramref name="expression"/> returned.</returns>
        public static Func<InputStream, ParseResult> WithAction(Func<InputStream, ParseResult> expression, Func<ParseResult, ParseResult> onMatched)
        {
            return input =>
                {
                    ParseResult result = expression(input);
                    if (result.Matched)
                    {
                        return onMatched(result);
                    }
                    return result;
                };
        }

        private static ParseResult MatchAndConsumeCurrentChar(InputStream input)
        {
            var result = new ParseResult(input.CurrentChar.ToString());
            input.Consume(1);
            return result;
        }
    }
}
