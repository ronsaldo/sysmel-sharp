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
            Assert.AreEqual(tokens.Count, 1);
            Assert.AreEqual(tokens[0].kind, TokenKind.EndOfSource);
        }

        [TestMethod]
        public void TestWhiteString()
        {
            List<Token> tokens = ScanString("    ");
            Assert.AreEqual(tokens.Count, 1);
            Assert.AreEqual(tokens[0].kind, TokenKind.EndOfSource);
        }

        [TestMethod]
        public void TestSingleLineComment()
        {
            List<Token> tokens = ScanString(" ## Test Comment");
            Assert.AreEqual(tokens.Count, 1);
            Assert.AreEqual(tokens[0].kind, TokenKind.EndOfSource);
        }

        [TestMethod]
        public void TestMultiLineComment()
        {
            List<Token> tokens = ScanString(" #* Test Comment *#");
            Assert.AreEqual(tokens.Count, 1);
            Assert.AreEqual(tokens[0].kind, TokenKind.EndOfSource);
        }
    }
}
