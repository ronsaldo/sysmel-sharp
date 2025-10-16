using System.Diagnostics;

namespace LibSysmelSharp
{
    public class Intrinsics
    {
        static private Intrinsics? uniqueInstance;
        private Dictionary<string, Value> globalIntrinsicDictionary = new Dictionary<string, Value>();
        private Dictionary<SysmelType, MutableBoxType> mutableBoxTypeCache= new Dictionary<SysmelType, MutableBoxType> ();
        private Dictionary<SysmelType, PointerType> pointerTypeCache = new Dictionary<SysmelType, PointerType>();
        private Dictionary<SysmelType, ReferenceType> refTypeCache = new Dictionary<SysmelType, ReferenceType>();
        private Dictionary<SysmelType, ListType> listTypeCache = new Dictionary<SysmelType, ListType>();

        public UniverseType TypeUniverse0;

        public NominalType VoidType;
        public NominalType UndefinedObjectType;
        public NominalType DynamicType;

        public NominalType BooleanType;
        public NominalType CharacterType;
        public NominalType IntegerType;
        public NominalType FloatType;
        public NominalType StringType;
        public NominalType SymbolType;

        public NominalType ParseTreeNodeType;
        public NominalType MacroContextType;
        public NominalType MetaBuilderType;

        public NominalType Char8Type;
        public NominalType Char16Type;
        public NominalType Char32Type;

        public NominalType UInt8Type;
        public NominalType UInt16Type;
        public NominalType UInt32Type;
        public NominalType UInt64Type;

        public NominalType Int8Type;
        public NominalType Int16Type;
        public NominalType Int32Type;
        public NominalType Int64Type;

        public NominalType Float32Type;
        public NominalType Float64Type;

        public ListType DynamicListType; // TODO: Use an array type.
        public ListType ByteArrayType; // TODO: Use an array type.
        public AssociationType DynamicImmutableDictionaryAssociationType;
        public ImmutableDictionaryType DynamicImmutableDictionaryType;

        public Queue<Value> pendingAnalysisQueue = new Queue<Value> ();

        public void AddValueWithPendingAnalysis(Value value)
        {
            pendingAnalysisQueue.Enqueue(value);
        }

        public void RunPendingAnalysis()
        {
            while(pendingAnalysisQueue.Count > 0)
            {
                Value value = pendingAnalysisQueue.Dequeue();
                value.EnsureAnalysis();
            }
        }

        public Value? LookIntrinsicGlobal(string symbol)
        {
            if (!globalIntrinsicDictionary.ContainsKey(symbol))
                return null;
            return globalIntrinsicDictionary[symbol];
        }

        NominalType CreateNominalType(string name)
        {
            NominalType type = new NominalType(null, name);
            globalIntrinsicDictionary[name] = type;
            return type;
        }
        void RegisterIntrinsicValue(string name, Value value)
        {
            globalIntrinsicDictionary[name] = value;
        }

        internal Intrinsics()
        {
            TypeUniverse0 = new UniverseType(null, 0);
            globalIntrinsicDictionary["Type"] = TypeUniverse0;

            VoidType            = CreateNominalType("Void");
            UndefinedObjectType = CreateNominalType("UndefinedObject");
            DynamicType         = CreateNominalType("Dynamic");

            BooleanType   = CreateNominalType("Boolean");
            CharacterType = CreateNominalType("Character");
            IntegerType   = CreateNominalType("Integer");
            FloatType     = CreateNominalType("Float");
            StringType    = CreateNominalType("String");
            SymbolType    = CreateNominalType("Symbol");

            Char8Type  = CreateNominalType("Char8");
            Char16Type = CreateNominalType("Char16");
            Char32Type = CreateNominalType("Char32");

            UInt8Type  = CreateNominalType("UInt8");
            UInt16Type = CreateNominalType("UInt16");
            UInt32Type = CreateNominalType("UInt32");
            UInt64Type = CreateNominalType("UInt64");

            Int8Type  = CreateNominalType("Int8");
            Int16Type = CreateNominalType("Int16");
            Int32Type = CreateNominalType("Int32");
            Int64Type = CreateNominalType("Int64");

            Float32Type = CreateNominalType("Float32");
            Float64Type = CreateNominalType("Float64");

            ParseTreeNodeType = CreateNominalType("ParseTreeNode");
            MacroContextType  = CreateNominalType("MacroContext");
            MetaBuilderType = CreateNominalType("MetaBuilder");

            DynamicListType = GetOrCreateListOfType(DynamicType);
            ByteArrayType = GetOrCreateListOfType(UInt8Type);
            DynamicImmutableDictionaryAssociationType = new AssociationType(null, SymbolType, DynamicType);
            DynamicImmutableDictionaryType = new ImmutableDictionaryType(null, DynamicImmutableDictionaryAssociationType);

            RegisterIntrinsicValue("false", BooleanValue.GetFalse());
            RegisterIntrinsicValue("true",  BooleanValue.GetTrue());
            RegisterIntrinsicValue("nil",   NilValue.GetSingleton());
            RegisterIntrinsicValue("void",  VoidValue.GetSingleton());

            RegisterIntrinsicValue("let", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new LetMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("function", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new FunctionMetaBuilder(sourcePosition);
            }));
        }

        public static void Reset()
        {
            uniqueInstance = null;
        }

        public static Intrinsics GetSingleton()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new Intrinsics();
                uniqueInstance.RegisterPrimitives();
            }

            return uniqueInstance;
        }

        public MutableBoxType GetOrCreateMutableBoxType(SysmelType baseType)
        {
            if(mutableBoxTypeCache.ContainsKey(baseType))
                return mutableBoxTypeCache[baseType];

            MutableBoxType newBoxType = new MutableBoxType(null, baseType);
            mutableBoxTypeCache[baseType] = newBoxType;
            return newBoxType;
        }

        public PointerType GetOrCreatePointerType(SysmelType baseType)
        {
            if (pointerTypeCache.ContainsKey(baseType))
                return pointerTypeCache[baseType];

            PointerType newPointerType = new PointerType(null, baseType);
            pointerTypeCache[baseType] = newPointerType;
            return newPointerType;
        }

        public ReferenceType GetOrCreateReferenceType(SysmelType baseType)
        {
            if (refTypeCache.ContainsKey(baseType))
                return refTypeCache[baseType];

            ReferenceType referenceType = new ReferenceType(null, baseType);
            refTypeCache[baseType] = referenceType;
            return referenceType;
        }

        public ListType GetOrCreateListOfType(SysmelType baseType)
        {
            if (listTypeCache.ContainsKey(baseType))
                return listTypeCache[baseType];

            ListType listType = new ListType(null, baseType);
            listTypeCache[baseType] = listType;
            return listType;
        }

        public void AddMethod(AbstractFunction method)
        {
            Debug.Assert(method.name != null);
            globalIntrinsicDictionary[method.name] = method;
        }

        void RegisterControlFlowMacros()
        {
            AddMethod(new PrimitiveFunction("if:then:else:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode trueExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode falseExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeIfSelectionNode(context.sourcePosition, condition, trueExpression, falseExpression);
                }));
            AddMethod(new PrimitiveFunction("if:then:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode trueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeIfSelectionNode(context.sourcePosition, condition, trueExpression, null);
                }));

            AddMethod(new PrimitiveFunction("return:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode resultExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeReturnNode(context.sourcePosition, resultExpression);
                }));
        }

        void RegisterDefinitionMacros()
        {
            AddMethod(new PrimitiveFunction("let:with:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression= (ParseTreeNode)arguments[1];
                    ParseTreeNode initialValueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeVariableDefinitionNode(context.sourcePosition, nameExpression, null, initialValueExpression, false);
                }));
            AddMethod(new PrimitiveFunction("let:mutableWith:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode initialValueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeVariableDefinitionNode(context.sourcePosition, nameExpression, null, initialValueExpression, true);
                }));
        }

        void RegisterIntegerTypePrimitives(NominalType integerType)
        {
            integerType.AddMethod(new PrimitiveFunction("negated", false,
                new SimpleFunctionType(null, [ integerType ], integerType),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, -arguments[0].AsInteger(), integerType);
                }));

            integerType.AddMethod(new PrimitiveFunction("+", false,
                new SimpleFunctionType(null, [ integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue + rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("-", false,
                new SimpleFunctionType(null, [ integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue - rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("*", false,
                new SimpleFunctionType(null, [integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue * rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("//", false,
                new SimpleFunctionType(null, [integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue / rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("%", false,
                new SimpleFunctionType(null, [integerType, integerType], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue % rightValue, integerType);
                }));

            integerType.AddMethod(new PrimitiveFunction("=", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue == rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("~=", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue != rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("<", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue < rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("<=", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue <= rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction(">", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue > rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction(">=", false,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue >= rightValue);
                }));
        }
        void RegisterStringPrimitives()
        {
            StringType.AddMethod(new PrimitiveFunction("=", false,
                new SimpleFunctionType(null, [StringType, StringType], StringType),
                (Value[] arguments) => {
                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return BooleanValue.GetWithValue(leftString == rightString);
                }));
            StringType.AddMethod(new PrimitiveFunction("~=", false,
                new SimpleFunctionType(null, [StringType, StringType], StringType),
                (Value[] arguments) => {
                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return BooleanValue.GetWithValue(leftString != rightString);
                }));
            StringType.AddMethod(new PrimitiveFunction("--", false,
                new SimpleFunctionType(null, [StringType, StringType], StringType),
                (Value[] arguments) => {
                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return new StringValue(null, leftString + rightString, StringType);
                }));
            StringType.AddMethod(new PrimitiveFunction("size", false,
                new SimpleFunctionType(null, [StringType], IntegerType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    return new IntegerValue(null, str.Length, IntegerType);
                }));

        }

        void RegisterTypePrimitives()
        {
            TypeUniverse0.AddMethod(new PrimitiveFunction("list", false,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).ListOf();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("pointer", false,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).Pointer();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("ref", false,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).Ref();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("=>", false,
                new SimpleFunctionType(null, [TypeUniverse0, TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    SysmelType argumentType = (SysmelType)arguments[0];
                    SysmelType resultType = (SysmelType)arguments[1];
                    return argumentType.MakeSimpleFunctionWithResultType(resultType);
                }));
        }

        void RegisterIOPrimitive()
        {
            AddMethod(new PrimitiveFunction("write", false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.Write(arguments[0].AsString());
                    return VoidValue.GetSingleton();
                }));
            AddMethod(new PrimitiveFunction("writeLine", false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.WriteLine(arguments[0].AsString());
                    return VoidValue.GetSingleton();
                }));

            AddMethod(new PrimitiveFunction("print", false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.Write(arguments[0].ToString());
                    return VoidValue.GetSingleton();
                }));
            AddMethod(new PrimitiveFunction("printLine", false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.WriteLine(arguments[0].ToString());
                    return VoidValue.GetSingleton();
                }));
        }

        void RegisterPrimitives()
        {
            RegisterControlFlowMacros();
            RegisterDefinitionMacros();
            RegisterStringPrimitives();
            RegisterTypePrimitives();
            RegisterIOPrimitive();
            RegisterIntegerTypePrimitives(IntegerType);
        }
    }
}
