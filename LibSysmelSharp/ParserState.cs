using System.Collections.Generic;
using System.Diagnostics;

namespace LibSysmelSharp
{
    internal class ParserState
    {
        public Token[] tokens;
        public int tokenCount;
        public int position;

        public ParserState(Token[] tokens)
        {
            this.tokens = tokens;
            this.tokenCount = tokens.Length;
            this.position = 0;
        }

        public bool AtEnd()
        {
            return position >= tokenCount;
        }

        public void Advance()
        {
            Debug.Assert(position < tokenCount);
            position = position + 1;
        }

        public Token Next()
        {
            Debug.Assert(position < tokenCount);
            return tokens[position++];
        }

        public Token? Peek()
        {
            return PeekAt(0);
        }

        public Token? PeekAt(int offset)
        {
            int peekPosition = position + offset;
            if (peekPosition < tokenCount)
                return tokens[peekPosition];
            return null;
        }

        public TokenKind PeekKind()
        {
            return PeekKindAt(0);
        }

        public TokenKind PeekKindAt(int offset)
        {
            int peekPosition = position + offset;
            if (peekPosition < tokenCount)
                return tokens[peekPosition].kind;

            return TokenKind.EndOfSource;
        }

        public int GetMemento()
        {
            return position;
        }

        public void RestoreMemento(int memento)
        {
            position = memento;
        }

        private SourcePosition PreviousSourcePosition()
        {
            Debug.Assert(position > 0);
            return tokens[position - 1].sourcePosition;
        }

        private SourcePosition CurrentSourcePosition()
        {
            if (position < tokenCount)
                return tokens[position].sourcePosition;

            Debug.Assert(tokenCount > 0);
            return tokens[tokenCount - 1].sourcePosition;
        }

        public SourcePosition SourcePositionFrom(int startingPosition)
        {
            Debug.Assert(startingPosition < tokenCount);
            SourcePosition startPosition = tokens[startingPosition].sourcePosition;
            if (position > 0)
            {
                SourcePosition endSourcePosition = PreviousSourcePosition();
                return startPosition.To(endSourcePosition);
            }
            else
            {
                SourcePosition endSourcePosition = CurrentSourcePosition();
                return startPosition.Until(endSourcePosition);
            }
        }

        public ParseTreeErrorNode MakeErrorAtCurrentPosition(string errorMessage)
        {
            ParseTreeErrorNode errorNode = new ParseTreeErrorNode();
            errorNode.sourcePosition = CurrentSourcePosition();
            errorNode.message = errorMessage;
            return errorNode;
        }

        public ParseTreeErrorNode AdvanceWithExpectedError(string errorMessage)
        {
            if(PeekKind() == TokenKind.Error)
            {
                Token errorToken = Next();
                
                ParseTreeErrorNode errorNode = new ParseTreeErrorNode();
                errorNode.sourcePosition = errorToken.sourcePosition;
                errorNode.message = errorToken.errorMessage;
                return errorNode;
            }

            if (!AtEnd())
                Advance();

            {
                ParseTreeErrorNode errorNode = new ParseTreeErrorNode();
                errorNode.sourcePosition = CurrentSourcePosition();
                errorNode.message = errorMessage;
                return errorNode;
            }
        }
    }
}
