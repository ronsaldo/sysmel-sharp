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
    }
}
