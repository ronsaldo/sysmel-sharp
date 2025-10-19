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
            LibSysmelSharp.Environment environment = new LexicalEnvironment(new NamespaceEnvironment(Intrinsics.GetSingleton().globalNamespace, new EmptyEnvironment()));
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
        public void TestEmptyByteArray()
        {
            Value value = EvaluateString("#[]");
            Assert.IsTrue(value.IsByteArrayValue());

            ByteArrayValue listValue = (ByteArrayValue)value;
            Assert.AreEqual(0, listValue.elements.Length);
        }

        [TestMethod]
        public void TestByteArray()
        {
            Value value = EvaluateString("#[1 . 2]");
            Assert.IsTrue(value.IsByteArrayValue());

            ByteArrayValue listValue = (ByteArrayValue)value;
            Assert.AreEqual(2, listValue.elements.Length);

            Assert.AreEqual(1, listValue.elements[0]);
            Assert.AreEqual(2, listValue.elements[1]);
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
        public void TestSwitchWithCases()
        {
            Value value = EvaluateString("switch: 2 withCases: #{0: -1. 2:-4 . _: 42}");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(-4, value.AsInteger());

            value = EvaluateString("switch: 5 withCases: #{0: -1. 2:-4 . _: 42}");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
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
        public void TestFunctionDump()
        {
            EvaluateString("function two() => Integer := 2. two dump");
        }

        [TestMethod]
        public void TestFunctionMacro()
        {
            Value value = EvaluateString("Function: #two withArguments: () resultType: Integer definition: 2");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("Function: #two withArguments: () resultType: Integer definition: 2. two()");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());

        }

        [TestMethod]
        public void TestFunctionMacro2()
        {
            Value value = EvaluateString("Function: #identity withArguments: (value: Integer) resultType: Integer definition: value");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("Function: #identity withArguments: (value: Integer) resultType: Integer definition: value. identity(2)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestFunctionMacro3()
        {
            Value value = EvaluateString("Function: #identity withArguments: (value: Integer) definition: value");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("Function: #identity withArguments: (value: Integer) definition: value. identity(2)");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestFunctionMacro4()
        {
            Value value = EvaluateString("Function: #add withArguments: (first: Integer. second: Integer) resultType: Integer definition: first + second.");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.GetSysmelType().IsFunctionType());

            value = EvaluateString("Function: #add withArguments: (first: Integer. second: Integer) resultType: Integer definition: first + second. add(2. 3)");
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

        [TestMethod]
        public void TestDoWhileLoop()
        {
            Value reference = EvaluateString("let: #i mutableWith: 0. do: {i := i + 1} while: (i < 10). i");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(10, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestDoContinueWithWhileLoop()
        {
            Value reference = EvaluateString("let: #i mutableWith: 0. do: {} continueWith: {i := i + 1} while: (i < 10). i");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(10, loadedValue.AsInteger());

        }
        [TestMethod]
        public void TestWhileLoop()
        {
            Value reference = EvaluateString("let: #i mutableWith: 0. while: i < 10 do: {i := i + 1}. i");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(10, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestWhileContinueWithLoop()
        {
            Value reference = EvaluateString("let: #i mutableWith: 0. while: i < 10 do: {} continueWith: {i := i + 1}. i");
            Assert.IsTrue(reference.IsReferenceLikeValue());
            Assert.AreEqual(Intrinsics.GetSingleton().IntegerType.Ref(), reference.GetSysmelType());

            Value loadedValue = reference.LoadValue();
            Assert.IsTrue(loadedValue.IsIntegerValue());
            Assert.AreEqual(10, loadedValue.AsInteger());
        }

        [TestMethod]
        public void TestEnumType()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsEnumType());
            
            EnumType enumType = (EnumType)value;
            Assert.AreEqual(1, enumType.nameAndValues["First"].AsInteger());
            Assert.AreEqual(2, enumType.nameAndValues["Second"].AsInteger());
            Assert.AreEqual(3, enumType.nameAndValues["Third"].AsInteger());

        }

        [TestMethod]
        public void TestEnumTypeAccess()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third");
            Assert.IsTrue(value.IsEnumValue());
            Assert.AreEqual(3, value.AsInteger());
        }

        [TestMethod]
        public void TestEnumTypeAccess2()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third value");
            Assert.IsFalse(value.IsEnumValue());
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(3, value.AsInteger());
        }

        [TestMethod]
        public void TestEnumTypeEquality()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third = TestEnum Third");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.IsTrue(value.AsBoolean());

            value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third = TestEnum Second");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.IsFalse(value.AsBoolean());
        }

        [TestMethod]
        public void TestEnumTypeEquality2()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third ~= TestEnum Second");
            Assert.IsTrue(value.IsBooleanValue());
            Assert.IsTrue(value.AsBoolean());
        }

        [TestMethod]
        public void TestEnumTypeHash()
        {
            Value value = EvaluateString("Enum: #TestEnum withBaseType: Integer values: #{First: 1. Second: 2. Third:}. TestEnum Third hash");
            Assert.IsTrue(value.IsIntegerValue());
        }

        [TestMethod]
        public void TestMessageCascade()
        {
            Value value = EvaluateString("42 negated; + 2");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(44, value.AsInteger());
        }

        [TestMethod]
        public void TestMessageCascade2()
        {
            Value value = EvaluateString("42 negated; + 2; yourself");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(42, value.AsInteger());
        }

        [TestMethod]
        public void TestAnonRecordType()
        {
            Value value = EvaluateString("RecordWithFields: #{first: Dynamic. second: Dynamic}");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsRecordType());

            RecordType recordType = (RecordType)value;
            Assert.AreEqual(2, recordType.fields.Length);

            Assert.AreEqual("first", recordType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[0].type);

            Assert.AreEqual("second", recordType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[1].type);
        }


        [TestMethod]
        public void TestRecordType()
        {
            Value value = EvaluateString("Record: #MyDynamicPair withFields: #{first: Dynamic. second: Dynamic}");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsRecordType());

            RecordType recordType = (RecordType)value;
            Assert.AreEqual(2, recordType.fields.Length);

            Assert.AreEqual("first", recordType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[0].type);

            Assert.AreEqual("second", recordType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[1].type);
        }

        [TestMethod]
        public void TestRecordType2()
        {
            Value value = EvaluateString("Record: #MyDynamicPair withFields: #{first:. second:}");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsRecordType());

            RecordType recordType = (RecordType)value;
            Assert.AreEqual(2, recordType.fields.Length);

            Assert.AreEqual("first", recordType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[0].type);

            Assert.AreEqual("second", recordType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, recordType.fields[1].type);
        }

        [TestMethod]
        public void TestRecord()
        {
            Value value = EvaluateString("Record: #MyDynamicPair withFields: #{first: Dynamic. second: Dynamic}. MyDynamicPair(1. 2)");
            Assert.IsTrue(value.IsRecordValue());
        }

        [TestMethod]
        public void TestRecord2()
        {
            Value value = EvaluateString("Record: #MyDynamicPair withFields: #{first: Dynamic. second: Dynamic}. MyDynamicPair#{first: 1. second: 2}");
            Assert.IsTrue(value.IsRecordValue());
        }

        [TestMethod]
        public void TestRecordFieldAccess()
        {
            Value value = EvaluateString("Record: #MyPair withFields: #{first: Integer. second: Integer}. MyPair#{first: 1. second: 2} first");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(1, value.AsInteger());

            value = EvaluateString("Record: #MyPair withFields: #{first: Integer. second: Integer}. MyPair#{first: 1. second: 2} second");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(2, value.AsInteger());
        }

        [TestMethod]
        public void TestAnonClassDefinition()
        {
            Value value = EvaluateString("ClassWithFields: #{first: Dynamic. second: Dynamic}.");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsClassType());

            ClassType classType = (ClassType)value;
            Assert.AreEqual(2, classType.fields.Length);

            Assert.AreEqual("first", classType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[0].type);

            Assert.AreEqual("second", classType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[1].type);
        }
        [TestMethod]
        public void TestAnoncClassDefinition2()
        {
            Value value = EvaluateString("ClassWithSuperclass: Object fields: #{first: Dynamic. second: Dynamic}.");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsClassType());

            ClassType classType = (ClassType)value;
            Assert.AreEqual(2, classType.fields.Length);

            Assert.AreEqual("first", classType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[0].type);

            Assert.AreEqual("second", classType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[1].type);
        }


        [TestMethod]
        public void TestClassDefinition()
        {
            Value value = EvaluateString("Class: #MyClass withFields: #{first: Dynamic. second: Dynamic}.");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsClassType());

            ClassType classType = (ClassType)value;
            Assert.AreEqual(2, classType.fields.Length);

            Assert.AreEqual("first", classType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[0].type);

            Assert.AreEqual("second", classType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[1].type);
        }

        [TestMethod]
        public void TestClassDefinition2()
        {
            Value value = EvaluateString("Class: #MyClass withSuperclass: Object fields: #{first: Dynamic. second: Dynamic}.");
            Assert.IsTrue(value.IsType());
            Assert.IsTrue(((SysmelType)value).IsClassType());

            ClassType classType = (ClassType)value;
            Assert.AreEqual(2, classType.fields.Length);

            Assert.AreEqual("first", classType.fields[0].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[0].type);

            Assert.AreEqual("second", classType.fields[1].name);
            Assert.AreEqual(Intrinsics.GetSingleton().DynamicType, classType.fields[1].type);
        }

        [TestMethod]
        public void TestClassInstance()
        {
            Value value = EvaluateString("Class: #MyClass withSuperclass: Object fields: #{first: Int32. second: Int64}. MyClass basicNew");
            Assert.IsTrue(value.IsClassInstanceValue());

            ClassInstanceValue instance = (ClassInstanceValue)value;
            Assert.AreEqual(2, instance.elements.Length);

            Assert.AreEqual(0, instance.elements[0].AsInteger());
            Assert.AreEqual(0, instance.elements[1].AsInteger());
        }

        [TestMethod]
        public void TestClassInstance2()
        {
            Value value = EvaluateString("Class: #MyClass withSuperclass: Object fields: #{first: Int32. second: Int64}. MyClass new");
            Assert.IsTrue(value.IsClassInstanceValue());

            ClassInstanceValue instance = (ClassInstanceValue)value;
            Assert.AreEqual(2, instance.elements.Length);

            Assert.AreEqual(0, instance.elements[0].AsInteger());
            Assert.AreEqual(0, instance.elements[1].AsInteger());
        }

        [TestMethod]
        public void TestMethod()
        {
            Value value = EvaluateString("!{identity self}");
            Assert.IsTrue(value.IsFunctionValue());
        }

        [TestMethod]
        public void TestMethod2()
        {
            Value value = EvaluateString("!{(Integer)identity :: Integer. self}");
            Assert.IsTrue(value.IsFunctionValue());
        }

        [TestMethod]
        public void TestMethod3()
        {
            Value value = EvaluateString("!{(Integer)identity self}");
            Assert.IsTrue(value.IsFunctionValue());
        }

        [TestMethod]
        public void TestMethod4()
        {
            Value value = EvaluateString("!{identity :: Integer. self}");
            Assert.IsTrue(value.IsFunctionValue());
        }

        [TestMethod]
        public void TestClassAddMethod()
        {
            Value value = EvaluateString("Class: #MyClass withSuperclass: Object fields: #{first: Integer. second: Integer}. MyClass!{sum first + second}. MyClass new sum");
            Assert.IsTrue(value.IsIntegerValue());
            Assert.AreEqual(0, value.AsInteger());
        }

        [TestMethod]
        public void TestNamespace()
        {
            Value value = EvaluateString("namespace: #TestNamespace definition: {}");
            Assert.IsTrue(value.IsNamespace());

            Namespace nspace = (Namespace)value;
            Assert.AreEqual("TestNamespace", nspace.name);
        }

        [TestMethod]
        public void TestNamespaceExport()
        {
            Value value = EvaluateString("namespace: #TestNamespace definition: {public: [Class: #TestClass withFields: #{}]}");
            Assert.IsTrue(value.IsNamespace());

            Namespace nspace = (Namespace)value;
            Assert.AreEqual("TestNamespace", nspace.name);
            Assert.AreEqual(1, nspace.children.Count);

            Value child = nspace.children[0];
            Assert.IsTrue(child.IsType());
            Assert.IsTrue( ((SysmelType)child).IsClassType()); ;

            ClassType childClass = (ClassType)child;
            Assert.AreEqual("TestClass", childClass.name);
        }

        [TestMethod]
        public void TestNamespaceAccess()
        {
            Value value = EvaluateString("namespace: #TestNamespace definition: {public: [Class: #TestClass withFields: #{}]} . TestNamespace TestClass");
            Assert.IsTrue(value.IsType());

            SysmelType typeValue = (SysmelType)value;
            Assert.IsTrue(typeValue.IsClassType());;
            Assert.AreEqual("TestClass", typeValue.GetName());
        }

        [TestMethod]
        public void TestNamespaceMetaBuilder()
        {
            Value value = EvaluateString("namespace TestNamespace definition: {}");
            Assert.IsTrue(value.IsNamespace());

            Namespace nspace = (Namespace)value;
            Assert.AreEqual("TestNamespace", nspace.name);
        }

        [TestMethod]
        public void TestNamespaceMetaBuilder2()
        {
            Value value = EvaluateString("namespace TestNamespace := {}");
            Assert.IsTrue(value.IsNamespace());

            Namespace nspace = (Namespace)value;
            Assert.AreEqual("TestNamespace", nspace.name);
        }

        [TestMethod]
        public void TestNamespaceMetabuilderAccess()
        {
            Value value = EvaluateString("namespace TestNamespace := {public: [Class: #TestClass withFields: #{}]} . TestNamespace TestClass");
            Assert.IsTrue(value.IsType());

            SysmelType typeValue = (SysmelType)value;
            Assert.IsTrue(typeValue.IsClassType()); ;
            Assert.AreEqual("TestClass", typeValue.GetName());
        }


        [TestMethod]
        public void TestGenericBox()
        {
            Value value = EvaluateString("public: [ForAll: (V: Type) in: #MyBox withDefinition: (RecordWithFields: #{boxedValue: V})]. MyBox");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.IsTemplate());

            Value firstValue = EvaluateString("MyBox(Integer)");
            Assert.IsTrue(firstValue.IsType());
            SysmelType valueType = (SysmelType)firstValue;
            Assert.IsTrue(valueType.IsRecordType());

            Value secondValue = EvaluateString("MyBox(Integer)");
            Assert.AreSame(firstValue, secondValue);
        }

        [TestMethod]
        public void TestGenericMethod()
        {
            Value templateValue = EvaluateString("public: [ForAll: (V: Type) in: #MyBox withDefinition: (RecordWithFields: #{boxedValue: V})]. MyBox");
            Assert.IsTrue(templateValue.IsFunctionValue());
            Assert.IsTrue(templateValue.IsTemplate());

            Value addMethodValue = EvaluateString("MyBox !{value :: V. boxedValue}");
            Assert.AreEqual(templateValue, addMethodValue);

            Value instanceValue = EvaluateString("MyBox(Integer)(42)");
            Assert.IsTrue(instanceValue.IsRecordValue());

            Value instanceValueAccess = EvaluateString("MyBox(Integer)(42) value");
            Assert.IsTrue(instanceValueAccess.IsIntegerValue());
            Assert.AreEqual(42, instanceValueAccess.AsInteger());
        }

        [TestMethod]
        public void TestGenericBoxMetabuilder()
        {
            Value value = EvaluateString("public: [forall(V: Type) MyBox := RecordWithFields: #{boxedValue: V}]. MyBox");
            Assert.IsTrue(value.IsFunctionValue());
            Assert.IsTrue(value.IsTemplate());

            Value firstValue = EvaluateString("MyBox(Integer)");
            Assert.IsTrue(firstValue.IsType());
            SysmelType valueType = (SysmelType)firstValue;
            Assert.IsTrue(valueType.IsRecordType());

            Value secondValue = EvaluateString("MyBox(Integer)");
            Assert.AreSame(firstValue, secondValue);
        }

        [TestMethod]
        public void TestGenericMethodMetabuilder()
        {
            Value templateValue = EvaluateString("public: [forall(V: Type) MyBox := RecordWithFields: #{boxedValue: V}]. MyBox");
            Assert.IsTrue(templateValue.IsFunctionValue());
            Assert.IsTrue(templateValue.IsTemplate());

            Value addMethodValue = EvaluateString("MyBox !{value :: V. boxedValue}");
            Assert.AreEqual(templateValue, addMethodValue);

            Value instanceValue = EvaluateString("MyBox(Integer)(42)");
            Assert.IsTrue(instanceValue.IsRecordValue());

            Value instanceValueAccess = EvaluateString("MyBox(Integer)(42) value");
            Assert.IsTrue(instanceValueAccess.IsIntegerValue());
            Assert.AreEqual(42, instanceValueAccess.AsInteger());
        }
    }
}
