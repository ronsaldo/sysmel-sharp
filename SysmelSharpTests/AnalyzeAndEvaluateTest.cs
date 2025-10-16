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

        [TestInitialize]
        public void TestInitialize()
        {
            Intrinsics.Reset();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Intrinsics.GetSingleton().RunPendingAnalysis();
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


        [TestMethod]
        public void TestIfThenExpression()
        {
            Value value = EvaluateString("if: false then: 0");
            Assert.IsTrue(value.IsVoidValue());

            value = EvaluateString("if: true then: 0");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(0, value.AsInteger());
        }

        [TestMethod]
        public void TestIfThenElseExpression()
        {
            Value value = EvaluateString("if: false then: 0 else: 1");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(1, value.AsInteger());

            value = EvaluateString("if: true then: 0 else: 1");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(0, value.AsInteger());
        }

        [TestMethod]
        public void TestLetWithMacro()
        {
            Value value = EvaluateString("let: #x with: 42. x");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestLetMutableWithMacro()
        {
            Value reference = EvaluateString("let: #x mutableWith: 42. x");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(42, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestLetMutableWithMacro2()
        {
            Value reference = EvaluateString("let: #x mutableWith: 42. x := 5. x");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(5, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestLetMutableWithMacro3()
        {
            Value value = EvaluateString("let: #x mutableWith: 42. x := 5. x + 7");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(12, value.AsInteger());
        }

        [TestMethod]
        public void TestLetMetabuilder()
        {
            Value value = EvaluateString("let var := 42. var");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestMutableLetMetabuilder()
        {
            Value reference = EvaluateString("let var mutable := 42. var");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(42, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestMutableLetMetabuilder2()
        {
            Value reference = EvaluateString("let var mutable := 42. var := 5. var");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(5, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestMutableLetMetabuilder3()
        {
            Value value = EvaluateString("let var mutable := 42. var := 5. var + 7");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(12, value.AsInteger());
        }

        [TestMethod]
        public void TestListType()
        {
            Value value = EvaluateString("Integer list");
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.ListOf(), value);
        }

        [TestMethod]
        public void TestPointerType()
        {
            Value value = EvaluateString("Integer pointer");
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Pointer(), value);
        }

        [TestMethod]
        public void TestRefType()
        {
            Value value = EvaluateString("Integer ref");
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), value);
        }

        [TestMethod]
        public void TestTuple()
        {
            Value value = EvaluateString("42, 32.5");
            Assert.IsTrue(value.IsTupleValue());

            TupleValue tuple = (TupleValue)value;
            Assert.AreEqual(2, tuple.elements.Length);

            Assert.IsTrue(tuple.elements[0].IsIntegerValue());
            Assert.AreEqual(42, tuple.elements[0].AsInteger());

            Assert.IsTrue(tuple.elements[1].IsFloatValue());
            Assert.AreEqual(32.5, tuple.elements[1].AsFloat());
        }


        [TestMethod]
        public void TestTupleType()
        {
            Value value = EvaluateString("Integer, Float");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsTupleType());

            TupleType tupleType = (TupleType)value;
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, tupleType.elements[0]);
            Assert.AreEqual(Intrinsics.GetSingleton().FloatType, tupleType.elements[1]);
        }

        [TestMethod]
        public void TestFunctionTypeNoArgs()
        {
            Value value = EvaluateString("Void => Integer");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsSimpleFunctionType());

            SimpleFunctionType functionType = (SimpleFunctionType)value;
            Assert.AreEqual(0, functionType.argumentTypes.Length);

            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, functionType.resultType);
        }

        [TestMethod]
        public void TestFunctionType()
        {
            Value value = EvaluateString("Integer => Integer");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsSimpleFunctionType());

            SimpleFunctionType functionType = (SimpleFunctionType)value;
            Assert.AreEqual(1, functionType.argumentTypes.Length);
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, functionType.argumentTypes[0]);

            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, functionType.resultType);
        }

        [TestMethod]
        public void TestFunctionTypeTwoArgs()
        {
            Value value = EvaluateString("(Integer, Float) => Integer");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsSimpleFunctionType());

            SimpleFunctionType functionType = (SimpleFunctionType)value;
            Assert.AreEqual(2, functionType.argumentTypes.Length);
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, functionType.argumentTypes[0]);
            Assert.AreEqual(Intrinsics.GetSingleton().FloatType, functionType.argumentTypes[1]);

            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType, functionType.resultType);
        }

        [TestMethod]
        public void TestFunctionTypeEquality()
        {
            Value firstValue = EvaluateString("(Integer, Float) => Integer");
            Value secondValue = EvaluateString("(Integer, Float) => Integer");
            Assert.AreEqual(firstValue, secondValue);
        }

        [TestMethod]
        public void TestTupleTypeEquality()
        {
            Value firstValue = EvaluateString("Integer, Float");
            Value secondValue = EvaluateString("Integer, Float");
            Assert.AreEqual(firstValue, secondValue);
        }

        [TestMethod]
        public void TestAssociation()
        {
            Value value = EvaluateString("1 : 2");
            Assert.IsTrue(value.IsAssociationValue());
            Assert.IsTrue(value.GetSysmelType().IsAssociationType());

            AssociationValue assoc = (AssociationValue)value;
            Assert.AreEqual(1, assoc.key.AsInteger());
            Assert.AreEqual(2, assoc.value.AsInteger());
        }

        [TestMethod]
        public void TestEmptyDictionaryExpression()
        {
            Value value = EvaluateString("#{}");
            Assert.IsTrue(value.IsImmutableDictionaryValue());
            Assert.IsTrue(value.GetSysmelType().IsImmutableDictionaryType());
        }

        [TestMethod]
        public void TestDictionaryExpression()
        {
            Value value = EvaluateString("#{first: 1}");
            Assert.IsTrue(value.IsImmutableDictionaryValue());
            Assert.IsTrue(value.GetSysmelType().IsImmutableDictionaryType());

            ImmutableDictionaryValue dict = (ImmutableDictionaryValue)value;
            Assert.AreEqual(1, dict.elements.Length);
        }

        [TestMethod]
        public void TestDictionaryExpression2()
        {
            Value value = EvaluateString("#{first: 1. Second: 2}");
            Assert.IsTrue(value.IsImmutableDictionaryValue());
            Assert.IsTrue(value.GetSysmelType().IsImmutableDictionaryType());

            ImmutableDictionaryValue dict = (ImmutableDictionaryValue)value;
            Assert.AreEqual(2, dict.elements.Length);
        }

        [TestMethod]
        public void TestDictionaryExpression3()
        {
            Value value = EvaluateString("#{first: 1. Second: 2. #third : 3}");
            Assert.IsTrue(value.IsImmutableDictionaryValue());
            Assert.IsTrue(value.GetSysmelType().IsImmutableDictionaryType());

            ImmutableDictionaryValue dict = (ImmutableDictionaryValue)value;
            Assert.AreEqual(3, dict.elements.Length);
        }

        [TestMethod]
        public void TestBlockClosure()
        {
            Value value = EvaluateString("{:x | x}");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());
        }

        [TestMethod]
        public void TestBlockClosure2()
        {
            Value value = EvaluateString("{:(Integer)x :: Integer | x}");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());
        }

        [TestMethod]
        public void TestBlockClosure3()
        {
            Value value = EvaluateString("{:x :: Integer | x}");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());
        }

        [TestMethod]
        public void TestBlockClosure4()
        {
            Value value = EvaluateString("{:(Integer)x | x}");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());
        }

        [TestMethod]
        public void TestBlockClosureEvaluation()
        {
            Value value = EvaluateString("{:x | x}(42)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestBlockClosureEvaluation2()
        {
            Value value = EvaluateString("{:(Integer)x :: Integer | x}(42)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestBlockClosureEvaluation3()
        {
            Value value = EvaluateString("{:x :: Integer | x}(42)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestBlockClosureEvaluation4()
        {
            Value value = EvaluateString("{:(Integer)x | x}(42)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestBlockClosureEvaluation5()
        {
            Value value = EvaluateString("{:(Integer)x :(Integer)y | x + y}(42. 3)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(45, value.AsInteger());
        }

        [TestMethod]
        public void TestInnerBlockClosure()
        {
            Value value = EvaluateString("{| {:x | x}}()");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());
        }

        [TestMethod]
        public void TestInnerBlockClosure2()
        {
            Value value = EvaluateString("{:x | {| x }}(42)()");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestFunction()
        {
            Value value = EvaluateString("function two() => Integer := 2");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("function two() => Integer := 2. two()");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestFunction2()
        {
            Value value = EvaluateString("function identity(value: Integer) => Integer := value");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("function identity(value: Integer) => Integer := value. identity(2)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestFunction3()
        {
            Value value = EvaluateString("function identity(value) := value");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("function identity(value) := value. identity(2)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestFunction4()
        {
            Value value = EvaluateString("function add(first: Integer. second: Integer) => Integer := first + second");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("function add(first: Integer. second: Integer) => Integer := first + second. add(2. 3)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(5, value.AsInteger());
        }

        [TestMethod]
        public void TestBlockClosureReturn()
        {
            Value value = EvaluateString("{:x | return: 42. x}");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("{:x | return: 42. x}(5)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestStringConcat()
        {
            Value value = EvaluateString("\"Hello \" -- \"World\"");
            Assert.IsTrue(value.IsStringValue());
            Assert.AreEqual("Hello World", value.AsString());
        }

        [TestMethod]
        public void TestStringSize()
        {
            Value value = EvaluateString("\"Hello \" size");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(6, value.AsInteger());
        }
    }
}
