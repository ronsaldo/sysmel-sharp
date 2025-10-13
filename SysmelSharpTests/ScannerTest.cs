using System.Collections.Generic;
using LibSysmelSharp;

namespace SysmelSharpTests
{
    [TestClass]
    public sealed class ScannerTest
    {
        public List<Token> ScanString(string sourceString)
        {
            SourceCode sourceCode = SourceCode.ForString("test", sourceString);
            return new Scanner().ScanSourceCode(sourceCode);
        }

        [TestMethod]
        public void TestEmptyString()
        {
            List<Token> tokens = ScanString("");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[0].kind);
        }

        [TestMethod]
        public void TestWhiteString()
        {
            List<Token> tokens = ScanString("    ");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[0].kind);
        }

        [TestMethod]
        public void TestSingleLineComment()
        {
            List<Token> tokens = ScanString(" ## Test Comment");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[0].kind);
        }

        [TestMethod]
        public void TestMultiLineComment()
        {
            List<Token> tokens = ScanString(" #* Test Comment *#");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[0].kind);
        }

        [TestMethod]
        public void TestIncompleteMultiLineComment()
        {
            List<Token> tokens = ScanString(" #* Test incomplete Comment");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Error, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestInteger()
        {
            List<Token> tokens = ScanString("42");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Integer, tokens[0].kind);
            Assert.AreEqual("42", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestNegativeInteger()
        {
            List<Token> tokens = ScanString("-42");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Integer, tokens[0].kind);
            Assert.AreEqual("-42", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestFloat()
        {
            List<Token> tokens = ScanString("42.5");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Float, tokens[0].kind);
            Assert.AreEqual("42.5", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestFloat2()
        {
            List<Token> tokens = ScanString("42.5e3");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Float, tokens[0].kind);
            Assert.AreEqual("42.5e3", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestFloat3()
        {
            List<Token> tokens = ScanString("42.5e-3");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Float, tokens[0].kind);
            Assert.AreEqual("42.5e-3", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestIdentifier()
        {
            List<Token> tokens = ScanString("identifier");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Identifier, tokens[0].kind);
            Assert.AreEqual("identifier", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestKeyword()
        {
            List<Token> tokens = ScanString("keyword:");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.Keyword, tokens[0].kind);
            Assert.AreEqual("keyword:", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestMultiKeyword()
        {
            List<Token> tokens = ScanString("multi:keyword:with:");
            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual(TokenKind.MultiKeyword, tokens[0].kind);
            Assert.AreEqual("multi:keyword:with:", tokens[0].GetText());
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }
    }
}
