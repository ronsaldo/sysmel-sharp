using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class Parser
    {
        internal ParserState state;

        public Parser()
        {
            state = new ParserState(new Token[0]);
        }

        internal static string ParseCEscapeCharacters(string cstring)
        {
            // TODO: Implement this method.
            return cstring;
        }

        private ParseTreeNode ParseLiteralCharacter()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.Character);

            string tokenText = token.GetText();
            string parsedTokenText = ParseCEscapeCharacters(tokenText.Substring(1, tokenText.Length - 2));

            return new ParseTreeLiteralCharacterNode(token.sourcePosition, parsedTokenText[0]);
        }

        private ParseTreeNode ParseLiteralInteger()
        {
            Token token = state.Next(); 
            Debug.Assert(token.kind == TokenKind.Integer);

            return new ParseTreeLiteralIntegerNode(token.sourcePosition, Int64.Parse(token.GetText()));
        }

        private ParseTreeNode ParseLiteralFloat()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.Float);

            return new ParseTreeLiteralFloatNode(token.sourcePosition, Double.Parse(token.GetText(), CultureInfo.InvariantCulture));
        }

        private ParseTreeNode ParseLiteralString()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.String);

            string tokenText = token.GetText();
            string parsedTokenText = ParseCEscapeCharacters(tokenText.Substring(1, tokenText.Length - 2));

            return new ParseTreeLiteralStringNode(token.sourcePosition, parsedTokenText);
        }
        private ParseTreeNode ParseLiteralSymbol()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.Symbol);

            string tokenText = token.GetText();
            string symbolText = tokenText;
            if(tokenText.StartsWith("#\""))
            {
                symbolText = ParseCEscapeCharacters(tokenText.Substring(2, tokenText.Length - 3));
            }
            else
            {
                Debug.Assert(tokenText.StartsWith("#"));
                symbolText = tokenText.Substring(1);
            }

            return new ParseTreeLiteralSymbolNode(token.sourcePosition, symbolText);

        }
        private ParseTreeNode ParseLiteral()
        {
            TokenKind kind = state.PeekKind();
            switch(kind)
            {
            case TokenKind.Character:
                return ParseLiteralCharacter();
            case TokenKind.Integer:
                return ParseLiteralInteger();
            case TokenKind.Float:
                return ParseLiteralFloat();
            case TokenKind.String:
                return ParseLiteralString();
            case TokenKind.Symbol:
                return ParseLiteralSymbol();
            default:
                return state.AdvanceWithExpectedError("Expected a literal");
            }
        }

        private ParseTreeNode ParseIdentifierReference()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.Identifier);

            return new ParseTreeIdentifierReferenceNode(token.sourcePosition, token.GetText());
        }

        private ParseTreeNode ParseParentheses()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.LeftParent);
            state.Advance();

            if(state.PeekKind() == TokenKind.RightParent)
            {
                state.Advance();
                return new ParseTreeTupleNode(state.SourcePositionFrom(startingPosition), new ParseTreeNode[0]);
            }

            ParseTreeNode expression = ParseExpressionSequenceUntilEndOrDelimiter(TokenKind.RightParent);
            return state.ExpectsAddingErrorNode(TokenKind.RightParent, "Missing right parenthesis.", expression);
        }

        private ParseTreeNode ParseTerm()
        {
            TokenKind kind = state.PeekKind();
            switch (kind)
            {
            case TokenKind.Identifier:
                return ParseIdentifierReference();
            case TokenKind.LeftParent:
                return ParseParentheses();
            default:
                return ParseLiteral();
            }
        }
        private ParseTreeNode ParseBinaryExpressionSequence()
        {
            return ParseTerm();
        }
        private ParseTreeNode ParseAssociationExpression()
        {
            return ParseBinaryExpressionSequence();
        }
        private ParseTreeNode ParseKeywordApplication()
        {
            return ParseTerm();
        }
        private ParseTreeNode ParseKeywordMessage()
        {
            return ParseTerm();
        }

        private ParseTreeNode ParseMessageCascade()
        {
            return ParseKeywordMessage();
        }

        private ParseTreeNode ParseLowPrecedenceExpression()
        {
            if (state.PeekKind() == TokenKind.Keyword)
                return ParseKeywordApplication();
            return ParseMessageCascade();
        }

        private ParseTreeNode ParseAssignmentExpression()
        {
            int startingPosition = state.position;
            ParseTreeNode assignedStore = ParseLowPrecedenceExpression();

            if (state.PeekKind() != TokenKind.Assignment)
                return assignedStore;

            state.Advance();
            ParseTreeNode assignedValue = ParseAssignmentExpression();

            return new ParseTreeAssignmentNode(state.SourcePositionFrom(startingPosition), assignedStore, assignedValue);
        }

        private ParseTreeNode ParseCommaExpression()
        {
            int startingPosition = state.position;
            ParseTreeNode firstElement = ParseAssignmentExpression();
            if (state.PeekKind() != TokenKind.Comma)
                return firstElement;

            List<ParseTreeNode> tupleElements = new List<ParseTreeNode>();
            tupleElements.Add(firstElement);

            while (state.PeekKind() == TokenKind.Comma)
            {
                state.Advance();
                ParseTreeNode nextElement = ParseAssignmentExpression();
                tupleElements.Add(nextElement);
            }

            return new ParseTreeTupleNode(state.SourcePositionFrom(startingPosition), tupleElements.ToArray());
        }

        private ParseTreeNode ParseExpresion()
        {
            return ParseCommaExpression();
        }

        private List<ParseTreeNode> ParseExpressionListUntilEndOrDelimiter(TokenKind delimiter)
        {
            List<ParseTreeNode> list = new List<ParseTreeNode> ();

            // Leading dots.
            while (state.PeekKind() == TokenKind.Dot)
                state.Advance();

            bool expectsExpression = true;
            while(!state.AtEnd() && state.PeekKind() != delimiter)
            {
                if (!expectsExpression)
                    list.Add(state.MakeErrorAtCurrentPosition("Expected dot before expression."));

                // Expression
                ParseTreeNode expression = ParseExpresion();
                list.Add(expression);

                // Trailing dots
                expectsExpression = false;
                while (state.PeekKind() == TokenKind.Dot)
                {
                    state.Advance();
                    expectsExpression = true;
                }

            }

            return list;
        }

        private ParseTreeNode ParseExpressionSequenceUntilEndOrDelimiter(TokenKind delimiter)
        {
            int startingPosition = state.position;
            List<ParseTreeNode> expressions = ParseExpressionListUntilEndOrDelimiter(delimiter);
            if (expressions.Count == 1)
                return expressions[0];

            ParseTreeSequenceNode node = new ParseTreeSequenceNode(state.SourcePositionFrom(startingPosition), expressions.ToArray());
            return node;
        }

        public ParseTreeNode ParseTokenList(SourceCode sourceCode, List<Token> tokenList)
        {
            state = new ParserState(tokenList.ToArray());
            return ParseExpressionSequenceUntilEndOrDelimiter(TokenKind.EndOfSource);
        }
    }
}
