using LibSysmelSharp;
using System.Collections.Generic;

namespace SysmelSharpTests
{
    [TestClass]
    public sealed class AnalyzeAndEvaluateTest
    {
        public Value EvaluateString(string sourceString)
        {
            SourceCode sourceCode = SourceCode.ForString("test", sourceString);
            List<Token> tokenList = new Scanner().ScanSourceCode(sourceCode);
            ParseTreeNode parseTree = new Parser().ParseTokenList(sourceCode, tokenList);
            LibSysmelSharp.Environment environment = new LexicalEnvironment(new IntrinsicsEnvironment(new EmptyEnvironment()));
            return parseTree.AnalyzeAndEvaluateInEnvironment(environment);
        }

        [TestMethod]
        public void TestLiteralCharacter()
        {
            Value value = EvaluateString("'A'");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual('A', value.AsInteger());
        }

        [TestMethod]
        public void TestLiteralInteger()
        {
            Value value = EvaluateString("42");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestLiteralFloat()
        {
            Value value = EvaluateString("42.5");
            Assert.IsTrue(value.IsFloatValue());
            Assert.AreEqual(42.5, value.AsFloat());
        }

        [TestMethod]
        public void TestLiteralString()
        {
            Value value = EvaluateString("\"Hello World\"");
            Assert.IsTrue(value.IsStringValue());
            Assert.AreEqual("Hello World", value.AsString());
        }

        [TestMethod]
        public void TestLiteralSymbol()
        {
            Value value = EvaluateString("#symbol");
            Assert.IsTrue(value.IsSymbolValue());
            Assert.AreEqual("symbol", value.AsSymbol());
        }
    }
}
