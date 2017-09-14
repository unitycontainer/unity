// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Unity.Configuration.ConfigurationHelpers
{
    internal class TypeNameParser : ParseBuilder
    {
        private class ParsedUnqualifiedName
        {
            public string Namespace;
            public string Rootname;
            public GenericParameters GenericParameters;
        }

        private class GenericParameters
        {
            public readonly List<TypeNameInfo> Parameters = new List<TypeNameInfo>();
            public bool IsOpenGeneric;
            public int Count { get { return Parameters.Count; } }
        }

        public static TypeNameInfo Parse(string typeName)
        {
            var input = new InputStream(typeName);

            ParseResult result = Sequence(Match_TypeName, EOF)(input);
            if (!result.Matched)
            {
                return null;
            }

            var typeNameResult = new SequenceResult(result)[0];
            return (TypeNameInfo)typeNameResult.ResultData;
        }

        private static void InitializeTypeNameInfo(ParsedUnqualifiedName from, TypeNameInfo to)
        {
            to.Name = from.Rootname;
            to.Namespace = from.Namespace;
            to.IsGenericType = from.GenericParameters != null;

            if (to.IsGenericType)
            {
                to.IsOpenGeneric = from.GenericParameters.IsOpenGeneric;
                if (to.IsOpenGeneric)
                {
                    for (int i = 0; i < from.GenericParameters.Count; ++i)
                    {
                        to.GenericParameters.Add(null);
                    }
                }
                else
                {
                    foreach (var genericParam in from.GenericParameters.Parameters)
                    {
                        to.GenericParameters.Add(genericParam);
                    }
                }
            }
        }

        // Parsing expressions from our grammar.
        private static ParseResult Match_TypeName(InputStream input)
        {
            var resultData = new TypeNameInfo();

            ParseResult result = Sequence(
                WithAction(Match_UnqualifiedName, r => InitializeTypeNameInfo((ParsedUnqualifiedName)r.ResultData, resultData)),
                ZeroOrOne(Sequence(Match_Comma, WithAction(Match_AssemblyName, r => resultData.AssemblyName = r.MatchedString))))(input);

            if (!result.Matched)
            {
                return result;
            }

            return new ParseResult(result.MatchedString, resultData);
        }

        private static ParseResult Match_AssemblyName(InputStream input)
        {
            return ParseBuilder.Sequence(Match_SimpleName, ParseBuilder.ZeroOrMore(ParseBuilder.Sequence(Match_Comma, Match_AssemblyNamePart)))(input);
        }

        private static ParseResult Match_UnqualifiedName(InputStream input)
        {
            var resultData = new ParsedUnqualifiedName();

            var result =
                Sequence(
                    WithAction(ZeroOrOne(Match_Namespace), r => resultData.Namespace = (string)r.ResultData),
                    WithAction(Match_RootName, r => resultData.Rootname = r.MatchedString),
                    WithAction(ZeroOrOne(Match_GenericParameters), r => resultData.GenericParameters = (GenericParameters)r.ResultData))(
                input);

            if (result.Matched)
            {
                return new ParseResult(result.MatchedString, resultData);
            }
            return result;
        }

        private static ParseResult Match_Namespace(InputStream input)
        {
            var ids = new List<string>();
            ParseResult result = OneOrMore(
                WithAction(Sequence(Match_Id, Match_Dot),
                    r => ids.Add(new SequenceResult(r)[0].MatchedString)))(input);

            if (result.Matched)
            {
                var ns = string.Join(".", ids.ToArray());
                return new ParseResult(result.MatchedString, ns);
            }

            return result;
        }

        private static ParseResult Match_RootName(InputStream input)
        {
            return ParseBuilder.Sequence(Match_Id, ParseBuilder.ZeroOrMore(Match_NestedName))(input);
        }

        private static ParseResult Match_NestedName(InputStream input)
        {
            return ParseBuilder.Sequence(Match_Plus, Match_Id)(input);
        }

        private static ParseResult Match_GenericParameters(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_ClosedGeneric, Match_OpenGeneric)(input);
        }

        private static ParseResult Match_OpenGeneric(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_CLRSyntax, Match_EmptyBrackets)(input);
        }

        private static ParseResult Match_CLRSyntax(InputStream input)
        {
            var resultData = new GenericParameters();
            ParseResult result = ParseBuilder.Sequence(Match_Backquote,
                WithAction(OneOrMore(Match_Digit),
                    r =>
                    {
                        resultData.IsOpenGeneric = true;
                        int numParameters = int.Parse(r.MatchedString, CultureInfo.InvariantCulture);
                        for (int i = 0; i < numParameters; ++i)
                        {
                            resultData.Parameters.Add(null);
                        }
                    }))(input);
            if (result.Matched)
            {
                return new ParseResult(result.MatchedString, resultData);
            }
            return result;
        }

        private static ParseResult Match_EmptyBrackets(InputStream input)
        {
            var resultData = new GenericParameters();
            ParseResult result = Sequence(Match_LeftBracket,
                WithAction(ZeroOrMore(Match_Comma), r =>
                    {
                        int numParameters = r.MatchedString.Length + 1;
                        resultData.IsOpenGeneric = true;
                        for (int i = 0; i < numParameters; ++i)
                        {
                            resultData.Parameters.Add(null);
                        }
                    }),
                Match_RightBracket)(input);
            if (result.Matched)
            {
                return new ParseResult(result.MatchedString, resultData);
            }
            return result;
        }

        private static ParseResult Match_ClosedGeneric(InputStream input)
        {
            var result = Sequence(ZeroOrOne(Match_CLRSyntax), Match_TypeParameters)(input);
            if (result.Matched)
            {
                var genericParameters = new GenericParameters();
                genericParameters.IsOpenGeneric = false;
                genericParameters.Parameters.AddRange((List<TypeNameInfo>)(new SequenceResult(result)[1].ResultData));
                return new ParseResult(result.MatchedString, genericParameters);
            }
            return result;
        }

        private static ParseResult Match_TypeParameters(InputStream input)
        {
            var typeParameters = new List<TypeNameInfo>();
            var result =
                Sequence(Match_LeftBracket,
                    WithAction(Match_GenericTypeParameter, StoreTypeParameter(typeParameters)),
                    ZeroOrMore(Sequence(Match_Comma, WithAction(Match_GenericTypeParameter, StoreTypeParameter(typeParameters)))), Match_RightBracket)(input);

            if (result.Matched)
            {
                return new ParseResult(result.MatchedString, typeParameters);
            }
            return result;
        }

        private static Action<ParseResult> StoreTypeParameter(ICollection<TypeNameInfo> l)
        {
            return r => l.Add((TypeNameInfo)r.ResultData);
        }

        private static ParseResult Match_GenericTypeParameter(InputStream input)
        {
            return ParseBuilder.FirstOf(
                ParseBuilder.WithAction(Match_UnqualifiedName, r =>
                    {
                        var result = new TypeNameInfo();
                        InitializeTypeNameInfo((ParsedUnqualifiedName)r.ResultData, result);
                        return new ParseResult(r.MatchedString, result);
                    }),
                ParseBuilder.WithAction(
                    ParseBuilder.Sequence(Match_LeftBracket, Match_TypeName, Match_RightBracket),
                        r => new SequenceResult(r)[1]))(
                input);
        }

        private static ParseResult Match_SimpleName(InputStream input)
        {
            return ParseBuilder.Sequence(Match_AssemblyNameStart, ParseBuilder.ZeroOrMore(Match_AssemblyNameContinuation), Match_Spacing)(input);
        }

        private static ParseResult Match_AssemblyNamePart(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_Culture, Match_Version, Match_PublicKey, Match_PublicKeyToken)(input);
        }

        private static ParseResult Match_Culture(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match("Culture"), Match_Spacing, Match_Equals, Match_LanguageTag)(input);
        }

        private static ParseResult Match_LanguageTag(InputStream input)
        {
            return ParseBuilder.Sequence(Match_LangTagPart, ParseBuilder.ZeroOrOne(ParseBuilder.Sequence(ParseBuilder.Match('-'), Match_LangTagPart)), Match_Spacing)(input);
        }

        private static ParseResult Match_LangTagPart(InputStream input)
        {
            var isAlpha = ParseBuilder.Match(ch => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(ch));
            var optAlpha = ParseBuilder.ZeroOrOne(isAlpha);
            return ParseBuilder.Sequence(isAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha,
                optAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha, optAlpha)(input);
        }

        private static ParseResult Match_Version(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match("Version"), Match_Spacing, Match_Equals, Match_VersionNumber, Match_Spacing)(input);
        }

        private static ParseResult Match_VersionNumber(InputStream input)
        {
            return
                ParseBuilder.Sequence(Match_Int, Match_Dot, Match_Int, Match_Dot, Match_Int, Match_Dot, Match_Int)(input);
        }

        private static ParseResult Match_PublicKey(InputStream input)
        {
            return
                ParseBuilder.Sequence(ParseBuilder.Match("PublicKey"), Match_Spacing, Match_Equals, ParseBuilder.OneOrMore(Match_HexDigit), Match_Spacing)(
                    input);
        }

        private static ParseResult Match_PublicKeyToken(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match("PublicKeyToken"), Match_Spacing, Match_Equals, Match_PublicKeyValue)(input);
        }

        private static ParseResult Match_PublicKeyValue(InputStream input)
        {
            var seq = Enumerable.Repeat<Func<InputStream, ParseResult>>(Match_HexDigit, 16)
                .Concat(new Func<InputStream, ParseResult>[] { Match_Spacing });

            return ParseBuilder.Sequence(seq.ToArray())(input);
        }

        private static ParseResult Match_AssemblyNameStart(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Not(Match_Dot), Match_AssemblyNameChar)(input);
        }

        private static ParseResult Match_AssemblyNameContinuation(InputStream input)
        {
            return Match_AssemblyNameChar(input);
        }

        private static ParseResult Match_AssemblyNameChar(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_QuotedChar, ParseBuilder.Match(ch => !"^/\\:?\"<>|,[]".Contains(ch)))(input);
        }

        private static ParseResult Match_Id(InputStream input)
        {
            return ParseBuilder.Sequence(Match_IdStart, ParseBuilder.ZeroOrMore(Match_IdContinuation))(input);
        }

        private static ParseResult Match_IdStart(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_IdNonAlpha, Match_IdAlpha)(input);
        }

        private static ParseResult Match_IdContinuation(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_IdNonAlpha, Match_IdAlphanumeric)(input);
        }

        private static ParseResult Match_IdAlpha(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_QuotedChar, ParseBuilder.Match(ch => char.IsLetter(ch)))(input);
        }

        private static ParseResult Match_IdAlphanumeric(InputStream input)
        {
            return ParseBuilder.FirstOf(Match_QuotedChar, ParseBuilder.Match(ch => char.IsLetterOrDigit(ch)))(input);
        }

        private static ParseResult Match_QuotedChar(InputStream input)
        {
            return ParseBuilder.WithAction(
                ParseBuilder.Sequence(Match_Backslash, ParseBuilder.Any),
                result =>
                {
                    string ch = new SequenceResult(result)[1].MatchedString;
                    return new ParseResult(ch);
                })(input);
        }

        private static ParseResult Match_IdNonAlpha(InputStream input)
        {
            return ParseBuilder.Match(ch => "_$@?".Contains(ch))(input);
        }

        private static ParseResult Match_Digit(InputStream input)
        {
            return ParseBuilder.Match(ch => char.IsDigit(ch))(input);
        }

        private static ParseResult Match_HexDigit(InputStream input)
        {
            return ParseBuilder.Match(ch => "0123456789ABCDEFabcdef".Contains(ch))(input);
        }

        private static ParseResult Match_Int(InputStream input)
        {
            return ParseBuilder.WithAction(ParseBuilder.Sequence(Match_Digit, ParseBuilder.ZeroOrMore(Match_Digit)),
                r => new ParseResult(r.MatchedString, int.Parse(r.MatchedString, CultureInfo.InvariantCulture)))(input);
        }

        private static ParseResult Match_LeftBracket(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match('['), Match_Spacing)(input);
        }

        private static ParseResult Match_RightBracket(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match(']'), Match_Spacing)(input);
        }

        private static ParseResult Match_Dot(InputStream input)
        {
            return ParseBuilder.Match('.')(input);
        }

        private static ParseResult Match_Backquote(InputStream input)
        {
            return ParseBuilder.Match('`')(input);
        }

        private static ParseResult Match_Plus(InputStream input)
        {
            return ParseBuilder.Match('+')(input);
        }

        private static ParseResult Match_Comma(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match(','), Match_Spacing)(input);
        }

        private static ParseResult Match_Backslash(InputStream input)
        {
            return ParseBuilder.Match('\\')(input);
        }

        private static ParseResult Match_Equals(InputStream input)
        {
            return ParseBuilder.Sequence(ParseBuilder.Match('='), Match_Spacing)(input);
        }

        private static ParseResult Match_Spacing(InputStream input)
        {
            return ParseBuilder.ZeroOrOne(Match_Space)(input);
        }

        private static ParseResult Match_Space(InputStream input)
        {
            return ParseBuilder.FirstOf(
                ParseBuilder.Match(' '),
                ParseBuilder.Match('\t'),
                Match_Eol)(input);
        }

        private static ParseResult Match_Eol(InputStream input)
        {
            return ParseBuilder.FirstOf(
                ParseBuilder.Match("\r\n"),
                ParseBuilder.Match('\r'),
                ParseBuilder.Match('\n'))(input);
        }
    }
}
