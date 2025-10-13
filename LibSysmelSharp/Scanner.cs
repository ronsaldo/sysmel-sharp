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

        private Token ScanDelimiters()
        {
            ScannerState initialState = state.Copy();

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
