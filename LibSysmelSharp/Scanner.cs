using System.Collections.Generic;

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

        private Token ScanSingleToken()
        {
            Token? whiteErrorToken = SkipWhite();
            if (whiteErrorToken != null)
                return whiteErrorToken;

            Token endOfSource = new Token();
            endOfSource.kind = TokenKind.EndOfSource;
            return endOfSource;
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
