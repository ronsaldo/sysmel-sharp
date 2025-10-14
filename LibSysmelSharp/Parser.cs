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

            ParseTreeLiteralCharacterNode literalNode = new ParseTreeLiteralCharacterNode();
            literalNode.sourcePosition = token.sourcePosition;
            literalNode.value = parsedTokenText[0];
            return literalNode;
        }

        private ParseTreeNode ParseLiteralInteger()
        {
            Token token = state.Next(); 
            Debug.Assert(token.kind == TokenKind.Integer);

            ParseTreeLiteralIntegerNode literalNode = new ParseTreeLiteralIntegerNode();
            literalNode.sourcePosition = token.sourcePosition;
            literalNode.value = Int64.Parse(token.GetText());
            return literalNode;
        }

        private ParseTreeNode ParseLiteralFloat()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.Float);

            ParseTreeLiteralFloatNode literalNode = new ParseTreeLiteralFloatNode();
            literalNode.sourcePosition = token.sourcePosition;
            literalNode.value = Double.Parse(token.GetText(), CultureInfo.InvariantCulture);
            return literalNode;
        }

        private ParseTreeNode ParseLiteralString()
        {
            Token token = state.Next();
            Debug.Assert(token.kind == TokenKind.String);

            string tokenText = token.GetText();
            string parsedTokenText = ParseCEscapeCharacters(tokenText.Substring(1, tokenText.Length - 2));

            ParseTreeLiteralStringNode literalNode = new ParseTreeLiteralStringNode();
            literalNode.sourcePosition = token.sourcePosition;
            literalNode.value = parsedTokenText;
            return literalNode;
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

            ParseTreeLiteralSymbolNode literalNode = new ParseTreeLiteralSymbolNode();
            literalNode.sourcePosition = token.sourcePosition;
            literalNode.value = symbolText;
            return literalNode;

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

            ParseTreeIdentifierReferenceNode identifierNode = new ParseTreeIdentifierReferenceNode();
            identifierNode.sourcePosition = token.sourcePosition;
            identifierNode.value = token.GetText();
            return identifierNode;
        }

        private ParseTreeNode ParseParentheses()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.LeftParent);
            state.Advance();

            if(state.PeekKind() == TokenKind.RightParent)
            {
                state.Advance();
                ParseTreeTupleNode tupleNode = new ParseTreeTupleNode();
                tupleNode.sourcePosition = state.SourcePositionFrom(startingPosition);
                tupleNode.elements = new ParseTreeNode[0];
                return tupleNode;
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
        private ParseTreeNode ParseAssignmentExpression()
        {
            return ParseTerm();
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

            ParseTreeTupleNode node = new ParseTreeTupleNode();
            node.sourcePosition = state.SourcePositionFrom(startingPosition);
            node.elements = tupleElements.ToArray();
            return node;
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
