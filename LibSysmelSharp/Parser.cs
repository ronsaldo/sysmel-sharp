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

        private ParseTreeNode ParseList()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.LeftBracket);
            state.Advance();

            List<ParseTreeNode> elements = ParseExpressionListUntilEndOrDelimiter(TokenKind.RightBracket);

            if (state.PeekKind() == TokenKind.RightBracket)
            {
                state.Advance();
            }
            else
            {
                elements.Add(state.MakeErrorAtCurrentPosition("Expected a right bracket."));
            }

            return new ParseTreeListNode(state.SourcePositionFrom(startingPosition), elements.ToArray());
        }

        private ParseTreeNode ParseByteArray()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.ByteArrayStart);
            state.Advance();

            List<ParseTreeNode> elements = ParseExpressionListUntilEndOrDelimiter(TokenKind.RightBracket);

            if (state.PeekKind() == TokenKind.RightBracket)
            {
                state.Advance();
            }
            else
            {
                elements.Add(state.MakeErrorAtCurrentPosition("Expected a right bracket."));
            }

            return new ParseTreeByteArrayNode(state.SourcePositionFrom(startingPosition), elements.ToArray());
        }

        ParseTreeArgumentDefinitionNode ParseBlockArgument()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.Colon);
            state.Advance();

            ParseTreeNode? typeExpression = null;
            if (state.PeekKind() == TokenKind.LeftParent)
                typeExpression = ParseParentheses();

            string? name = null;
            if (state.PeekKind() == TokenKind.Identifier)
                name = state.Next().GetText();

            return new ParseTreeArgumentDefinitionNode(state.SourcePositionFrom(startingPosition), name, typeExpression);
        }

        private ParseTreeFunctionalSignatureNode? ParseBlockFunctionalSignature()
        {
            bool hasFunctionalTypeComponents = false;
            int startingPosition = state.position;

            List<ParseTreeArgumentDefinitionNode> arguments = new List<ParseTreeArgumentDefinitionNode>();
            while(state.PeekKind() == TokenKind.Colon)
            {
                hasFunctionalTypeComponents = true;
                ParseTreeArgumentDefinitionNode argument = ParseBlockArgument();
                arguments.Add(argument);
            }

            ParseTreeNode? resultTypeExpression = null;
            if(state.PeekKind() == TokenKind.ColonColon)
            {
                state.Advance();
                resultTypeExpression = ParseUnaryPrefixExpression();
                hasFunctionalTypeComponents = true;
            }

            if(state.PeekKind() == TokenKind.Bar)
            {
                state.Advance();
                hasFunctionalTypeComponents = true;
            }

            if (!hasFunctionalTypeComponents)
                return null;

            return new ParseTreeFunctionalSignatureNode(state.SourcePositionFrom(startingPosition), arguments.ToArray(), resultTypeExpression, null);
        }

        private ParseTreeNode ParseBlock()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.LeftCurlyBracket);
            state.Advance();

            ParseTreeFunctionalSignatureNode? functionalSignature = ParseBlockFunctionalSignature();

            ParseTreeNode expression = ParseExpressionSequenceUntilEndOrDelimiter(TokenKind.RightCurlyBracket);
            ParseTreeNode expressionWithError = state.ExpectsAddingErrorNode(TokenKind.RightCurlyBracket, "Missing right curly bracket", expression);

            if (functionalSignature == null)
                return new ParseTreeLexicalBlockNode(state.SourcePositionFrom(startingPosition), expressionWithError);
            else
                return new ParseTreeBlockClosureNode(state.SourcePositionFrom(startingPosition), functionalSignature, expressionWithError);
        }

        private ParseTreeNode ParseImmutableDictionaryAssociation()
        {
            int startingPosition = state.position;
            ParseTreeNode? key = null;
            ParseTreeNode? value = null;

            if(state.PeekKind() == TokenKind.Keyword)
            {
                Token keyToken = state.Next();
                string keyTokenText = keyToken.GetText();
                string keyTokenSymbol = keyTokenText;
                if (keyTokenText.EndsWith(":"))
                    keyTokenSymbol = keyTokenText.Substring(0, keyTokenText.Length - 1);

                key = new ParseTreeLiteralSymbolNode(keyToken.sourcePosition, keyTokenSymbol);
                if (state.PeekKind() != TokenKind.Dot && state.PeekKind() != TokenKind.RightCurlyBracket)
                    value = ParseAssociationExpression();
            }
            else
            {
                key = ParseBinaryExpressionSequence();
                if (state.PeekKind() == TokenKind.Colon)
                {
                    state.Advance();
                    value = ParseAssociationExpression();
                }
            }

            return new ParseTreeAssociationNode(state.SourcePositionFrom(startingPosition), key, value);
        }
        private ParseTreeNode ParseImmutableDictionary()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.ImmutableDictionaryStart);
            state.Advance();

            // Chop the initial dots
            while (state.PeekKind() == TokenKind.Dot)
                state.Advance();

            bool expectsExpression = true;
            List<ParseTreeNode> elements = new List<ParseTreeNode> ();
            while(!state.AtEnd() && state.PeekKind() != TokenKind.RightCurlyBracket)
            {
                if(!expectsExpression)
                    elements.Add(state.MakeErrorAtCurrentPosition("Expected a dot before the association."));

                //  Parse the association
                ParseTreeNode expression = ParseImmutableDictionaryAssociation();
                elements.Add(expression);

                // Chop the trailing dots
                expectsExpression = false;
                while (state.PeekKind() == TokenKind.Dot)
                {
                    state.Advance();
                    expectsExpression = true;
                }
            }

            if(state.PeekKind() == TokenKind.RightCurlyBracket)
            {
                state.Advance();
            }
            else
            {
                elements.Add(state.MakeErrorAtCurrentPosition("Missing right bracket to close the dictionary."));
            }

            return new ParseTreeImmutableDictionaryNode(state.SourcePositionFrom(startingPosition), null, elements.ToArray());
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
            case TokenKind.LeftBracket:
                return ParseList();
            case TokenKind.LeftCurlyBracket:
                return ParseBlock();
            case TokenKind.ByteArrayStart:
                return ParseByteArray();
                case TokenKind.ImmutableDictionaryStart:
                return ParseImmutableDictionary();
            default:
                return ParseLiteral();
            }
        }

        bool IsUnaryPostfixToken(TokenKind kind)
        {
            return kind == TokenKind.Identifier || kind == TokenKind.LeftParent;
        }
        private ParseTreeNode ParseUnaryPostfixExpression()
        {
            int startingPosition = state.position;
            ParseTreeNode receiver = ParseTerm();

            while(IsUnaryPostfixToken(state.PeekKind()))
            {
                switch(state.PeekKind())
                {
                case TokenKind.Identifier:
                    {
                        Token token = state.Next();
                        string selector = token.GetText();
                        ParseTreeNode selectorNode = new ParseTreeLiteralSymbolNode(token.sourcePosition, selector);
                        receiver = new ParseTreeMessageSendNode(state.SourcePositionFrom(startingPosition), receiver, selectorNode, new ParseTreeNode[0]);
                    }
                    break;
                case TokenKind.LeftParent:
                    {
                        state.Advance();
                        List<ParseTreeNode> callArguments = ParseExpressionListUntilEndOrDelimiter(TokenKind.RightParent);
                        if (state.PeekKind() == TokenKind.RightParent)
                        {
                            state.Advance();
                        }
                        else
                        {
                            callArguments.Add(state.MakeErrorAtCurrentPosition("Expected a right parent."));
                        }

                        receiver = new ParseTreeApplicationNode(state.SourcePositionFrom(startingPosition), receiver, callArguments.ToArray());
                    }
                    break;
                }
            }

            return receiver;
        }
        private ParseTreeNode ParseQuoteExpression()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.Quote);
            state.Advance();

            ParseTreeNode expression = ParseUnaryPrefixExpression();
            return new ParseTreeQuoteNode(state.SourcePositionFrom(startingPosition), expression);
        }

        private ParseTreeNode ParseQuasiQuoteExpression()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.QuasiQuote);
            state.Advance();

            ParseTreeNode expression = ParseUnaryPrefixExpression();
            return new ParseTreeQuasiQuoteNode(state.SourcePositionFrom(startingPosition), expression);
        }

        private ParseTreeNode ParseQuasiUnquoteExpression()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.QuasiUnquote);
            state.Advance();

            ParseTreeNode expression = ParseUnaryPrefixExpression();
            return new ParseTreeQuasiUnquoteNode(state.SourcePositionFrom(startingPosition), expression);
        }

        private ParseTreeNode ParseSpliceExpression()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.Splice);
            state.Advance();

            ParseTreeNode expression = ParseUnaryPrefixExpression();
            return new ParseTreeSpliceNode(state.SourcePositionFrom(startingPosition), expression);
        }

        private ParseTreeNode ParseUnaryPrefixExpression()
        {
            switch(state.PeekKind())
            {
            case TokenKind.Quote: return ParseQuoteExpression();
            case TokenKind.QuasiQuote: return ParseQuasiQuoteExpression();
            case TokenKind.QuasiUnquote: return ParseQuasiUnquoteExpression();
            case TokenKind.Splice: return ParseSpliceExpression();
            }
            return ParseUnaryPostfixExpression();
        }

        private bool IsBinaryOperatorToken(TokenKind kind)
        {
            switch(kind)
            {
            case TokenKind.Operator:    return true;
            case TokenKind.Bar:         return true;
            case TokenKind.LessThan:    return true;
            case TokenKind.GreaterThan: return true;
            default: return false;
            }
        }

        private ParseTreeNode ParseBinaryExpressionSequence()
        {
            int startingPosition = state.position;
            ParseTreeNode firstOperand = ParseUnaryPrefixExpression();
            if (!IsBinaryOperatorToken(state.PeekKind()))
                return firstOperand;

            List<ParseTreeNode> operands = new List<ParseTreeNode>();
            operands.Add(firstOperand);
            
            while(IsBinaryOperatorToken(state.PeekKind()))
            {
                Token operatorToken = state.Next();
                operands.Add(new ParseTreeLiteralSymbolNode(operatorToken.sourcePosition, operatorToken.GetText()));

                ParseTreeNode operand = ParseUnaryPrefixExpression();
                operands.Add(operand);
            }

            return new ParseTreeBinaryExpressionSequence(state.SourcePositionFrom(startingPosition), operands.ToArray());
        }

        private ParseTreeNode ParseAssociationExpression()
        {
            int startingPosition = state.position;
            ParseTreeNode key = ParseBinaryExpressionSequence();
            if (state.PeekKind() != TokenKind.Colon)
                return key;

            state.Advance();
            ParseTreeNode value = ParseAssociationExpression();
            return new ParseTreeAssociationNode(state.SourcePositionFrom(startingPosition), key, value);
        }
        private ParseTreeNode ParseKeywordApplication()
        {
            int startingPosition = state.position;
            Debug.Assert(state.PeekKind() == TokenKind.Keyword);

            StringBuilder selectorText = new StringBuilder();
            List<ParseTreeNode> arguments = new List<ParseTreeNode>();

            while(state.PeekKind() == TokenKind.Keyword)
            {
                Token keywordToken = state.Next();
                selectorText.Append(keywordToken.GetText());
                ParseTreeNode argumentExpression = ParseAssociationExpression();
                arguments.Add(argumentExpression);
            }

            SourcePosition sourcePosition = state.SourcePositionFrom(startingPosition);
            ParseTreeIdentifierReferenceNode identifierRef = new ParseTreeIdentifierReferenceNode(sourcePosition, selectorText.ToString());
            return new ParseTreeApplicationNode(sourcePosition, identifierRef, arguments.ToArray());
        }

        private ParseTreeNode ParseKeywordMessage()
        {
            int startingPosition = state.position;
            ParseTreeNode receiver = ParseAssociationExpression();
            if (state.PeekKind() != TokenKind.Keyword)
                return receiver;

            StringBuilder selectorTextBuilder = new StringBuilder();
            List<ParseTreeNode> arguments = new List<ParseTreeNode>();
            while(state.PeekKind() == TokenKind.Keyword)
            {
                Token keyword = state.Next();
                ParseTreeNode argument = ParseAssociationExpression();

                selectorTextBuilder.Append(keyword.GetText());
                arguments.Add(argument);
            }

            SourcePosition sourcePosition = state.SourcePositionFrom(startingPosition);
            ParseTreeLiteralSymbolNode selectorNode = new ParseTreeLiteralSymbolNode(sourcePosition, selectorTextBuilder.ToString());
            return new ParseTreeMessageSendNode(sourcePosition, receiver, selectorNode, arguments.ToArray());
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
