
using System.Diagnostics;

namespace LibSysmelSharp
{
    internal class ScannerState
    {
        public SourceCode sourceCode;
        public string text = "";
        public int textSize;
        public int position;
        public int line;
        public int column;
        public bool isPreviousCR;

        public ScannerState(SourceCode sourceCode)
        {
            this.sourceCode = sourceCode;
        }

        public void Advance()
        {
            Debug.Assert(!AtEnd());

            int c = (int)text[position];
            position = position + 1;

            if (c == '\r')
            {
                line = line + 1;
                column = 1;
                isPreviousCR = true;
                return;
            }

            if(c == '\n')
            {
                if(isPreviousCR)
                {
                    isPreviousCR = false;
                    return;
                }

                line = line + 1;
                column = 1;
                isPreviousCR = false;
                return;
            }

            if(c == '\t')
            {
                column = (column + 4) / 4 * 4 + 1;
                return;
            }

            column = column + 1;
        }

        public void AdvanceWithCount(int count)
        {
            for (int i = 0; i < count; i++)
                Advance();
        }

        public bool AtEnd()
        {
            return position >= textSize;
        }

        public int PeekAt(int offset)
        {
            if (position + offset < textSize)
                return (int)text[position + offset];
            return -1;
        }

        public int Peek()
        {
            return PeekAt(0);
        }

        public ScannerState Copy()
        {
            ScannerState copy = new ScannerState(sourceCode);
            copy.text       = text;
            copy.textSize   = textSize;
            copy.position   = position;
            copy.line       = line;
            copy.column     = column;
            copy.isPreviousCR = isPreviousCR;
            return copy;
        }

        public Token MakeErrorToken(string errorMessage, ScannerState previousState)
        {
            SourcePosition sourcePosition = new SourcePosition(sourceCode);

            sourcePosition.startIndex = previousState.position;
            sourcePosition.endIndex = position;
            sourcePosition.startLine = previousState.line;
            sourcePosition.endLine = line;
            sourcePosition.startColumn = previousState.column;
            sourcePosition.endColumn = column;

            return new Token(TokenKind.Error, sourcePosition, errorMessage);
        }

        public Token MakeTokenStartingFrom(TokenKind kind, ScannerState previousState)
        {
            SourcePosition sourcePosition = new SourcePosition(sourceCode);

            sourcePosition.startIndex = previousState.position;
            sourcePosition.endIndex = position;
            sourcePosition.startLine = previousState.line;
            sourcePosition.endLine = line;
            sourcePosition.startColumn = previousState.column;
            sourcePosition.endColumn = column;

            return new Token(kind, sourcePosition);
        }

        static public ScannerState ForSourceCode(SourceCode sourceCode)
        {
            ScannerState state = new ScannerState(sourceCode);
            state.text = sourceCode.text;
            state.textSize = sourceCode.text.Length;
            state.position = 0;
            state.line = 1;
            state.column = 1;
            state.isPreviousCR = false;

            return state;
        }


    }
}
