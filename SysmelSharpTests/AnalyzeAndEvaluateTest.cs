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
        public void TestIntrinsicType()
        {
            Value value = EvaluateString("Integer");
            Assert.IsTrue(value.IsType());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, value);
        }

        [TestMethod]
        public void TestFalseValue()
        {
            Value value = EvaluateString("false");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(false, value.AsBoolean());
        }

        [TestMethod]
        public void TestTrueValue()
        {
            Value value = EvaluateString("true");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(true, value.AsBoolean());
        }

        [TestMethod]
        public void TestNilValue()
        {
            Value value = EvaluateString("nil");
            Assert.IsTrue(value.IsNilValue());
        }

        [TestMethod]
        public void TestVoidValue()
        {
            Value value = EvaluateString("void");
            Assert.IsTrue(value.IsVoidValue());
        }

        [TestMethod]
        public void TestLexicalBlock()
        {
            Value value = EvaluateString("{42}");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
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

        [TestMethod]
        public void TestEmptySequence()
        {
            Value value = EvaluateString("");
            Assert.IsTrue(value.IsVoidValue());

        }
        [TestMethod]
        public void TestSequence()
        {
            Value value = EvaluateString("1 . 2");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestEmptyList()
        {
            Value value = EvaluateString("[]");
            Assert.IsTrue(value.IsListValue());

            ListValue listValue = (ListValue)value;
            Assert.AreEqual(0, listValue.elements.Length);
        }

        [TestMethod]
        public void TestList()
        {
            Value value = EvaluateString("[1 . 2]");
            Assert.IsTrue(value.IsListValue());

            ListValue listValue = (ListValue)value;
            Assert.AreEqual(2, listValue.elements.Length);

            Assert.AreEqual(1, listValue.elements[0].AsInteger());
            Assert.AreEqual(2, listValue.elements[1].AsInteger());
        }

        [TestMethod]
        public void TestBinaryExpression()
        {
            Value value = EvaluateString("1 + 2");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(3, value.AsInteger());

            value = EvaluateString("2 - 3");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(-1, value.AsInteger());

            value = EvaluateString("2 * 3");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(6, value.AsInteger());

            value = EvaluateString("6 // 3");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());

            value = EvaluateString("7 % 3");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(1, value.AsInteger());
        }

        [TestMethod]
        public void TestBinaryExpression2()
        {
            Value value = EvaluateString("1 = 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(false, value.AsBoolean());

            value = EvaluateString("1 ~= 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(true, value.AsBoolean());

            value = EvaluateString("1 < 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(true, value.AsBoolean());

            value = EvaluateString("1 <= 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(true, value.AsBoolean());

            value = EvaluateString("1 > 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(false, value.AsBoolean());

            value = EvaluateString("1 >= 2");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.AreEqual(false, value.AsBoolean());
        }
    }
}
