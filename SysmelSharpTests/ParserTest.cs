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
    }
}
