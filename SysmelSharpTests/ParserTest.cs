using LibSysmelSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SysmelSharpTests
{
    [TestClass]
    public sealed class ParserTest
    {
        public ParseTreeNode ParseString(string sourceString)
        {
            SourceCode sourceCode = SourceCode.ForString("test", sourceString);
            List<Token> tokenList = new Scanner().ScanSourceCode(sourceCode);
            return new Parser().ParseTokenList(sourceCode, tokenList);
        }

        [TestMethod]
        public void TestApplication()
        {
            ParseTreeNode node = ParseString("x()");
            Assert.IsTrue(node.IsParseTreeApplicationNode());

            ParseTreeApplicationNode application = (ParseTreeApplicationNode)node;
            Assert.IsTrue(application.functional.IsParseTreeIdentifierReferenceNode());
            Assert.AreEqual(0, application.arguments.Length);
        }

        [TestMethod]
        public void TestApplication2()
        {
            ParseTreeNode node = ParseString("x(42)");
            Assert.IsTrue(node.IsParseTreeApplicationNode());

            ParseTreeApplicationNode application = (ParseTreeApplicationNode)node;
            Assert.IsTrue(application.functional.IsParseTreeIdentifierReferenceNode());
            Assert.AreEqual(1, application.arguments.Length);
        }

        [TestMethod]
        public void TestApplication3()
        {
            ParseTreeNode node = ParseString("x(42. 5)");
            Assert.IsTrue(node.IsParseTreeApplicationNode());

            ParseTreeApplicationNode application = (ParseTreeApplicationNode)node;
            Assert.IsTrue(application.functional.IsParseTreeIdentifierReferenceNode());
            Assert.AreEqual(2, application.arguments.Length);
        }

        [TestMethod]
        public void TestEmptySequence()
        {
            ParseTreeNode node = ParseString("");
            Assert.IsTrue(node.IsParseTreeSequenceNode());

            ParseTreeSequenceNode sequence = (ParseTreeSequenceNode)node;
            Assert.AreEqual(0, sequence.expressions.Length);
        }

        [TestMethod]
        public void TestLiteralCharacter()
        {
            ParseTreeNode node = ParseString("'A'");
            Assert.IsTrue(node.IsParseTreeLiteralCharacterNode());

            ParseTreeLiteralCharacterNode literal = (ParseTreeLiteralCharacterNode)node;
            Assert.AreEqual('A', literal.value);
        }

        [TestMethod]
        public void TestLiteralInteger()
        {
            ParseTreeNode node = ParseString("42");
            Assert.IsTrue(node.IsParseTreeLiteralIntegerNode());

            ParseTreeLiteralIntegerNode literal = (ParseTreeLiteralIntegerNode)node;
            Assert.AreEqual(42, literal.value);
        }

        [TestMethod]
        public void TestLiteralFloat()
        {
            ParseTreeNode node = ParseString("42.5");
            Assert.IsTrue(node.IsParseTreeLiteralFloatNode());

            ParseTreeLiteralFloatNode literal = (ParseTreeLiteralFloatNode)node;
            Assert.AreEqual(42.5, literal.value);
        }

        [TestMethod]
        public void TestLiteralString()
        {
            ParseTreeNode node = ParseString("\"Hello World\"");
            Assert.IsTrue(node.IsParseTreeLiteralStringNode());

            ParseTreeLiteralStringNode literal = (ParseTreeLiteralStringNode)node;
            Assert.AreEqual("Hello World", literal.value);
        }

        [TestMethod]
        public void TestLiteralSymbol()
        {
            ParseTreeNode node = ParseString("#symbol");
            Assert.IsTrue(node.IsParseTreeLiteralSymbolNode());

            ParseTreeLiteralSymbolNode literal = (ParseTreeLiteralSymbolNode)node;
            Assert.AreEqual("symbol", literal.value);
        }

        [TestMethod]
        public void TestLiteralSymbolString()
        {
            ParseTreeNode node = ParseString("#\"Hello World\"");
            Assert.IsTrue(node.IsParseTreeLiteralSymbolNode());

            ParseTreeLiteralSymbolNode literal = (ParseTreeLiteralSymbolNode)node;
            Assert.AreEqual("Hello World", literal.value);
        }

        [TestMethod]
        public void TestIdentifierReference()
        {
            ParseTreeNode node = ParseString("identifier");
            Assert.IsTrue(node.IsParseTreeIdentifierReferenceNode());

            ParseTreeIdentifierReferenceNode identNode = (ParseTreeIdentifierReferenceNode)node;
            Assert.AreEqual("identifier", identNode.value);
        }

        [TestMethod]
        public void TestSequenceWithTwoTerms()
        {
            ParseTreeNode node = ParseString("1 . 2");
            Assert.IsTrue(node.IsParseTreeSequenceNode());

            ParseTreeSequenceNode sequenceNode = (ParseTreeSequenceNode)node;
            Assert.AreEqual(2, sequenceNode.expressions.Length);

            Assert.IsTrue(sequenceNode.expressions[0].IsParseTreeLiteralIntegerNode());
            Assert.AreEqual(1, ((ParseTreeLiteralIntegerNode)sequenceNode.expressions[0]).value);

            Assert.IsTrue(sequenceNode.expressions[1].IsParseTreeLiteralIntegerNode());
            Assert.AreEqual(2, ((ParseTreeLiteralIntegerNode)sequenceNode.expressions[1]).value);
        }

        [TestMethod]
        public void TestParenthesesWithLiteral()
        {
            ParseTreeNode node = ParseString("(42)");
            Assert.IsTrue(node.IsParseTreeLiteralIntegerNode());

            ParseTreeLiteralIntegerNode literal = (ParseTreeLiteralIntegerNode)node;
            Assert.AreEqual(42, literal.value);
        }

        [TestMethod]
        public void TestEmptyList()
        {
            ParseTreeNode node = ParseString("[]");
            Assert.IsTrue(node.IsParseTreeListNode());

            ParseTreeListNode list = (ParseTreeListNode)node;
            Assert.AreEqual(0, list.elements.Length);
        }

        [TestMethod]
        public void TestList()
        {
            ParseTreeNode node = ParseString("[1 . 2]");
            Assert.IsTrue(node.IsParseTreeListNode());

            ParseTreeListNode list = (ParseTreeListNode)node;
            Assert.AreEqual(2, list.elements.Length);
        }

        [TestMethod]
        public void TestEmptyTuple()
        {
            ParseTreeNode node = ParseString("()");
            Assert.IsTrue(node.IsParseTreeTupleNode());

            ParseTreeTupleNode tuple = (ParseTreeTupleNode)node;
            Assert.AreEqual(0, tuple.elements.Length);
        }

        [TestMethod]
        public void TestTuple()
        {
            ParseTreeNode node = ParseString("1, 2, 3");
            Assert.IsTrue(node.IsParseTreeTupleNode());

            ParseTreeTupleNode tuple = (ParseTreeTupleNode)node;
            Assert.AreEqual(3, tuple.elements.Length);

            Assert.IsTrue(tuple.elements[0].IsParseTreeLiteralIntegerNode());
            Assert.AreEqual(1, ((ParseTreeLiteralIntegerNode)tuple.elements[0]).value);

            Assert.IsTrue(tuple.elements[1].IsParseTreeLiteralIntegerNode());
            Assert.AreEqual(2, ((ParseTreeLiteralIntegerNode)tuple.elements[1]).value);

            Assert.IsTrue(tuple.elements[2].IsParseTreeLiteralIntegerNode());
            Assert.AreEqual(3, ((ParseTreeLiteralIntegerNode)tuple.elements[2]).value);
        }

        [TestMethod]
        public void TestAssignmentExpression()
        {
            ParseTreeNode node = ParseString("x := 42");
            Assert.IsTrue(node.IsParseTreeAssignmentNode());

            ParseTreeAssignmentNode assignment = (ParseTreeAssignmentNode)node;
            Assert.IsTrue(assignment.store.IsParseTreeIdentifierReferenceNode());

            Assert.IsTrue(assignment.value.IsParseTreeLiteralIntegerNode());
        }

        [TestMethod]
        public void TestAssociationExpression()
        {
            ParseTreeNode node = ParseString("5 : 6");
            Assert.IsTrue(node.IsParseTreeAssociationNode());

            ParseTreeAssociationNode assoc = (ParseTreeAssociationNode)node;
            Assert.IsTrue(assoc.key.IsParseTreeLiteralIntegerNode()); ;
        }

        [TestMethod]
        public void TestKeywordApplication()
        {
            ParseTreeNode node = ParseString("if: true then: 1 else: 2");
            Assert.IsTrue(node.IsParseTreeApplicationNode());

            ParseTreeApplicationNode application= (ParseTreeApplicationNode)node;

            Assert.IsTrue(application.functional.IsParseTreeIdentifierReferenceNode());
            Assert.AreEqual(3, application.arguments.Length);
        }

        [TestMethod]
        public void TestKeywordMessageSend()
        {
            ParseTreeNode node = ParseString("42 perform: #+ with: 2");
            Assert.IsTrue(node.IsParseTreeMessageSendNode());

            ParseTreeMessageSendNode messageSend = (ParseTreeMessageSendNode)node;
            Assert.IsTrue(messageSend.receiver.IsParseTreeLiteralIntegerNode());
            Assert.IsTrue(messageSend.selector.IsParseTreeLiteralSymbolNode());
            Assert.AreEqual(2, messageSend.arguments.Length);
        }

        [TestMethod]
        public void TestBinaryExpression()
        {
            ParseTreeNode node = ParseString("1 + 2");
            Assert.IsTrue(node.IsParseTreeBinaryExpressionSequence());

            ParseTreeBinaryExpressionSequence expressionSequence = (ParseTreeBinaryExpressionSequence)node;
            Assert.AreEqual(3, expressionSequence.operands.Length);
        }

        [TestMethod]
        public void TestUnaryMessageSend()
        {
            ParseTreeNode node = ParseString("42 negated");
            Assert.IsTrue(node.IsParseTreeMessageSendNode());

            ParseTreeMessageSendNode send = (ParseTreeMessageSendNode)node;
            Assert.IsTrue(send.receiver.IsParseTreeLiteralIntegerNode());
            Assert.IsTrue(send.selector.IsParseTreeLiteralSymbolNode());
            Assert.AreEqual(0, send.arguments.Length);
        }

        [TestMethod]
        public void TestQuote()
        {
            ParseTreeNode node = ParseString("`'quote");
            Assert.IsTrue(node.IsParseTreeQuoteNode());
        }

        [TestMethod]
        public void TestQuasiQuote()
        {
            ParseTreeNode node = ParseString("``quote");
            Assert.IsTrue(node.IsParseTreeQuasiQuoteNode());
        }

        [TestMethod]
        public void TestQuasiUnquote()
        {
            ParseTreeNode node = ParseString("`,quote");
            Assert.IsTrue(node.IsParseTreeQuasiUnquoteNode());
        }

        [TestMethod]
        public void TestSplice()
        {
            ParseTreeNode node = ParseString("`@quote");
            Assert.IsTrue(node.IsParseTreeSpliceNode());
        }
    }
}
