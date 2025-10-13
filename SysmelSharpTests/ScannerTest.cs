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


        [TestMethod]
        public void TestDelimiters()
        {
            List<Token> tokens = ScanString("() [] {}");
            Assert.AreEqual(7, tokens.Count);

            Assert.AreEqual(TokenKind.LeftParent, tokens[0].kind);
            Assert.AreEqual(TokenKind.RightParent, tokens[1].kind);

            Assert.AreEqual(TokenKind.LeftBracket, tokens[2].kind);
            Assert.AreEqual(TokenKind.RightBracket, tokens[3].kind);

            Assert.AreEqual(TokenKind.LeftCurlyBracket, tokens[4].kind);
            Assert.AreEqual(TokenKind.RightCurlyBracket, tokens[5].kind);

            Assert.AreEqual(TokenKind.EndOfSource, tokens[6].kind);
        }

        [TestMethod]
        public void TestDelimiters2()
        {
            List<Token> tokens = ScanString("; , . ... : :: :=");
            Assert.AreEqual(8, tokens.Count);

            Assert.AreEqual(TokenKind.Semicolon, tokens[0].kind);
            Assert.AreEqual(TokenKind.Comma, tokens[1].kind);
            Assert.AreEqual(TokenKind.Dot, tokens[2].kind);
            Assert.AreEqual(TokenKind.Ellipsis, tokens[3].kind);
            Assert.AreEqual(TokenKind.Colon, tokens[4].kind);
            Assert.AreEqual(TokenKind.ColonColon, tokens[5].kind);
            Assert.AreEqual(TokenKind.Assignment, tokens[6].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[7].kind);
        }

        [TestMethod]
        public void TestDelimiters3()
        {
            List<Token> tokens = ScanString("#[ #{ #( !{");
            Assert.AreEqual(5, tokens.Count);

            Assert.AreEqual(TokenKind.ByteArrayStart, tokens[0].kind);
            Assert.AreEqual(TokenKind.ImmutableDictionaryStart, tokens[1].kind);
            Assert.AreEqual(TokenKind.LiteralArrayStart, tokens[2].kind);
            Assert.AreEqual(TokenKind.MethodStart, tokens[3].kind);

            Assert.AreEqual(TokenKind.EndOfSource, tokens[4].kind);
        }

        [TestMethod]
        public void TestMacroOperators()
        {
            List<Token> tokens = ScanString("`' `` `, `@");
            Assert.AreEqual(5, tokens.Count);

            Assert.AreEqual(TokenKind.Quote, tokens[0].kind);
            Assert.AreEqual(TokenKind.QuasiQuote, tokens[1].kind);
            Assert.AreEqual(TokenKind.QuasiUnquote, tokens[2].kind);
            Assert.AreEqual(TokenKind.Splice, tokens[3].kind);

            Assert.AreEqual(TokenKind.EndOfSource, tokens[4].kind);
        }

        [TestMethod]
        public void TestOperators()
        {
            List<Token> tokens = ScanString("| < > + *");
            Assert.AreEqual(6, tokens.Count);

            Assert.AreEqual(TokenKind.Bar, tokens[0].kind);
            Assert.AreEqual(TokenKind.LessThan, tokens[1].kind);
            Assert.AreEqual(TokenKind.GreaterThan, tokens[2].kind);
            Assert.AreEqual(TokenKind.Operator, tokens[3].kind);
            Assert.AreEqual(TokenKind.Operator, tokens[4].kind);

            Assert.AreEqual(TokenKind.EndOfSource, tokens[5].kind);
        }

        [TestMethod]
        public void TestCharacter()
        {
            List<Token> tokens = ScanString("'A'");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Character, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestString()
        {
            List<Token> tokens = ScanString("\"Hello World\"");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.String, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestSymbolIdentifier()
        {
            List<Token> tokens = ScanString("#identifier");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Symbol, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestSymbolOperator()
        {
            List<Token> tokens = ScanString("#+");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Symbol, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestSymbolString()
        {
            List<Token> tokens = ScanString("#\"String Symbol\"");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Symbol, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestSymbolKeyword()
        {
            List<Token> tokens = ScanString("#keyword:");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Symbol, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }

        [TestMethod]
        public void TestSymbolMultiKeyword()
        {
            List<Token> tokens = ScanString("#multi:keyword:with:");
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(TokenKind.Symbol, tokens[0].kind);
            Assert.AreEqual(TokenKind.EndOfSource, tokens[1].kind);
        }
    }
}
