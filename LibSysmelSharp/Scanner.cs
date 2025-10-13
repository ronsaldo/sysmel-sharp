using System.Collections.Generic;
using System.Resources;

namespace LibSysmelSharp
{
    public class Scanner
    {
        internal ScannerState state;

        public Scanner()
        {
            state = new ScannerState();
        }

        private Token? SkipWhite()
        {
            bool hasSeenComment = false;
            do
            {
                hasSeenComment = false;

                // Skip white spaces.
                while (!state.AtEnd() && state.Peek() <= ' ')
                    state.Advance();

                // Comment
                if(state.Peek() == '#')
                {
                    if(state.PeekAt(1) == '#')
                    {
                        // Single line comment.
                        hasSeenComment = true;
                        state.AdvanceWithCount(2);

                        while (!state.AtEnd() && state.Peek() != '\r' && state.Peek() != '\n')
                            state.Advance();

                    }
                    else if(state.PeekAt(1) == '*')
                    {
                        ScannerState startingState = state.Copy();

                        // Multi line comment.
                        hasSeenComment = true;
                        state.AdvanceWithCount(2);

                        bool hasCommentEnd = false;
                        while(!state.AtEnd() && !hasCommentEnd)
                        {
                            hasCommentEnd = state.PeekAt(0) == '*' && state.PeekAt(1) == '#';
                            if (hasCommentEnd)
                                state.AdvanceWithCount(2);
                            else
                                state.Advance();
                        }

                        if (!hasCommentEnd)
                            return state.MakeErrorToken("Incomplete multi-line comment.", startingState);
                    }
                }

            } while (hasSeenComment);

            return null;
        }

        private bool IsDigit(int c)
        {
            return '0' <= c && c <= '9';
        }

        private bool IsIdentifierStart(int c)
        {
            return ('A' <= c && c <= 'Z') ||
                   ('a' <= c && c <= 'z') ||
                   '_' == c;
        }

        private bool IsIdentifierMiddle(int c)
        {
            return IsIdentifierStart(c) || IsDigit(c);
        }
        private bool IsE(int c)
        {
            return c == 'e' || c == 'E';
        }

        private bool IsSign(int c)
        {
            return c == '-' || c == '+';
        }

        private bool IsOperatorCharacter(int c)
        {
            if (c < 0)
                return false;
            return "+-/\\*~<>=@%|&?!^".IndexOf((char)c) >= 0;
        }

        private Token ScanOperator()
        {
            ScannerState initialState = state.Copy();
            int c = state.Peek();
            state.Advance();

            if(!IsOperatorCharacter(state.Peek()))
            {
                if(c == '|')
                    return state.MakeTokenStartingFrom(TokenKind.Bar, initialState);
                else if (c == '<')
                    return state.MakeTokenStartingFrom(TokenKind.LessThan, initialState);
                else if (c == '>')
                    return state.MakeTokenStartingFrom(TokenKind.GreaterThan, initialState);
            }

            while (IsOperatorCharacter(state.Peek()))
                state.Advance();

            return state.MakeTokenStartingFrom(TokenKind.Operator, initialState);
        }

        private Token ScanDelimiters()
        {
            ScannerState initialState = state.Copy();
            int c = state.Peek();
            switch(c)
            {
            case '(':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.LeftParent, initialState);
            case ')':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.RightParent, initialState);
            case '[':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.LeftBracket, initialState);
            case ']':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.RightBracket, initialState);
            case '{':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.LeftCurlyBracket, initialState);
            case '}':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.RightCurlyBracket, initialState);
            case ';':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.Semicolon, initialState);
            case ',':
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.Comma, initialState);
            case '!':
                if(state.PeekAt(1) == '{')
                {
                    state.AdvanceWithCount(2);
                    return state.MakeTokenStartingFrom(TokenKind.MethodStart, initialState);
                }
                break;
            case '.':
                state.Advance();
                if (state.Peek() == '.' && state.PeekAt(1) == '.')
                {
                    state.AdvanceWithCount(2);
                    return state.MakeTokenStartingFrom(TokenKind.Ellipsis, initialState);
                }
                return state.MakeTokenStartingFrom(TokenKind.Dot, initialState);
            case ':':
                state.Advance();
                if (state.Peek() == ':')
                {
                    state.Advance();
                    return state.MakeTokenStartingFrom(TokenKind.ColonColon, initialState);
                }
                else if (state.Peek() == '=')
                {
                    state.Advance();
                    return state.MakeTokenStartingFrom(TokenKind.Assignment, initialState);
                }
                return state.MakeTokenStartingFrom(TokenKind.Colon, initialState);
            case '`':
                {
                    int c2 = state.PeekAt(1);
                    if(c2 == '\'')
                    {
                        state.AdvanceWithCount(2);
                        return state.MakeTokenStartingFrom(TokenKind.Quote, initialState);
                    }
                    if (c2 == '`')
                    {
                        state.AdvanceWithCount(2);
                        return state.MakeTokenStartingFrom(TokenKind.QuasiQuote, initialState);
                    }
                    if (c2 == ',')
                    {
                        state.AdvanceWithCount(2);
                        return state.MakeTokenStartingFrom(TokenKind.QuasiUnquote, initialState);
                    }
                    if (c2 == '@')
                    {
                        state.AdvanceWithCount(2);
                        return state.MakeTokenStartingFrom(TokenKind.Splice, initialState);
                    }
                }
                break;
            default:
                break;
            }

            if (IsOperatorCharacter(c))
                return ScanOperator();

            state.Advance();
            return state.MakeErrorToken("Unknown character", initialState);
        }

        private Token ScanNumber()
        {
            ScannerState initialState = state.Copy();

            // Sign
            if (state.Peek() == '-')
                state.Advance();

            // Integer part.
            while (IsDigit(state.Peek()))
                state.Advance();

            // Fractional part
            if(state.Peek() == '.' && IsDigit(state.PeekAt(1)))
            {
                state.Advance();
                while (IsDigit(state.Peek()))
                    state.Advance();

                if(IsE(state.Peek()))
                {
                    if(IsDigit(state.PeekAt(1)) || IsSign(state.PeekAt(1)) && IsDigit(state.PeekAt(2)))
                    {
                        // Exponent part.
                        state.AdvanceWithCount(2);
                        while (IsDigit(state.Peek()))
                            state.Advance();
                    }
                }

                return state.MakeTokenStartingFrom(TokenKind.Float, initialState);
            }

            return state.MakeTokenStartingFrom(TokenKind.Integer, initialState);
        }
        private bool ScanAdvanceKeyword()
        {
            if (!IsIdentifierStart(state.Peek()))
                return false;

            ScannerState memento = state.Copy();

            while(IsIdentifierMiddle(state.Peek()))
                state.Advance();

            if (state.Peek() == ':')
            {
                state.Advance();
                return true;
            }
            else
            {
                state = memento;
                return false;
            }
        }

        private Token ScanIdentifierOrKeyword()
        {
            ScannerState initialState = state.Copy();

            while (IsIdentifierMiddle(state.Peek()))
                state.Advance();

            if(state.Peek() == ':')
            {
                state.Advance();
                bool isMultiKeyword = false;
                bool hasAdvanced = true;
                while(hasAdvanced)
                {
                    hasAdvanced = ScanAdvanceKeyword();
                    isMultiKeyword = isMultiKeyword || hasAdvanced;
                }

                return state.MakeTokenStartingFrom(isMultiKeyword ? TokenKind.MultiKeyword : TokenKind.Keyword, initialState);
            }

            return state.MakeTokenStartingFrom(TokenKind.Identifier, initialState);
        }

        private Token ScanPoundSequence()
        {
            ScannerState initialState = state.Copy();
            state.Advance();

            int c1 = state.Peek();
            if(IsIdentifierStart(c1))
            {
                state.Advance();
                while (IsIdentifierMiddle(state.Peek()))
                    state.Advance();

                if(state.Peek() == ':')
                {
                    state.Advance();
                    bool hasAdvanced = true;

                    while(hasAdvanced)
                        hasAdvanced = ScanAdvanceKeyword();
                }

                return state.MakeTokenStartingFrom(TokenKind.Symbol, initialState);
            }

            if(IsOperatorCharacter(c1))
            {
                while (IsOperatorCharacter(state.Peek()))
                    state.Advance();
                
                return state.MakeTokenStartingFrom(TokenKind.Symbol, initialState);
            }

            if(c1 == '"')
            {
                state.Advance();

                while (!state.AtEnd() && state.Peek() != '"')
                {
                    if (state.Peek() == '\\' && state.PeekAt(1) >= 0)
                        state.AdvanceWithCount(2);
                    else
                        state.Advance();
                }

                if (state.Peek() != '"')
                    return state.MakeErrorToken("Incomplete symbol string literal", initialState);

                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.Symbol, initialState);
            }

            if(c1 == '[')
            {
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.ByteArrayStart, initialState);
            }
            if (c1 == '{')
            {
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.ImmutableDictionaryStart, initialState);
            }
            if (c1 == '(')
            {
                state.Advance();
                return state.MakeTokenStartingFrom(TokenKind.LiteralArrayStart, initialState);
            }

            return state.MakeErrorToken("Unknown character", initialState);
        }
        private Token ScanCharacter()
        {
            ScannerState initialState = state.Copy();
            state.Advance();

            while (!state.AtEnd() && state.Peek() != '\'')
            {
                if (state.Peek() == '\\' && state.PeekAt(1) >= 0)
                    state.AdvanceWithCount(2);
                else
                    state.Advance();
            }

            if (state.Peek() != '\'')
                return state.MakeErrorToken("Incomplete character literal", initialState);

            state.Advance();
            return state.MakeTokenStartingFrom(TokenKind.Character, initialState);
        }

        private Token ScanString()
        {
            ScannerState initialState = state.Copy();
            state.Advance();

            while(!state.AtEnd() && state.Peek() != '"')
            {
                if(state.Peek() == '\\' && state.PeekAt(1) >= 0)
                    state.AdvanceWithCount(2);
                else
                    state.Advance();
            }

            if (state.Peek() != '"')
                return state.MakeErrorToken("Incomplete string literal", initialState);
            
            state.Advance();
            return state.MakeTokenStartingFrom(TokenKind.String, initialState);
        }

        private Token ScanSingleToken()
        {
            Token? whiteErrorToken = SkipWhite();
            if (whiteErrorToken != null)
                return whiteErrorToken;

            if(state.AtEnd())
            {
                Token endOfSource = new Token();
                endOfSource.kind = TokenKind.EndOfSource;
                return endOfSource;
            }

            int c = state.Peek();

            // Numbers
            if ((c == '-' && IsDigit(state.PeekAt(1)))
                || IsDigit(c))
                return ScanNumber();

            // Identifiers
            if (IsIdentifierStart(c))
                return ScanIdentifierOrKeyword();

            // Pound sequence.
            if (c == '#')
                return ScanPoundSequence();

            // Characters
            if (c == '\'')
                return ScanCharacter();

            // Strings
            if (c == '"')
                return ScanString();

            return ScanDelimiters();
        }

        public List<Token> ScanSourceCode(SourceCode sourceCode)
        {
            List<Token> tokenList = new List<Token>();
            state = ScannerState.ForSourceCode(sourceCode);
            Token? scannedToken = null;
            do
            {
                scannedToken = ScanSingleToken();
                tokenList.Add(scannedToken);
            } while (scannedToken.kind != TokenKind.EndOfSource);

            return tokenList;
        }

    }
}
