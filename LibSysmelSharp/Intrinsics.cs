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

        public ListType DynamicListType; // TODO: Use an array type.

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

            ParseTreeNodeType = CreateNominalType("ParseTreeNode");
            MacroContextType  = CreateNominalType("MacroContextType");

            DynamicListType = GetOrCreateListOfType(DynamicType);

            RegisterIntrinsicValue("false", BooleanValue.GetFalse());
            RegisterIntrinsicValue("true",  BooleanValue.GetTrue());
            RegisterIntrinsicValue("nil",   NilValue.GetSingleton());
            RegisterIntrinsicValue("void",  VoidValue.GetSingleton());
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
        }

        void RegisterPrimitives()
        {
            RegisterControlFlowMacros();
            RegisterDefinitionMacros();
            RegisterTypePrimitives();
            RegisterIntegerTypePrimitives(IntegerType);
        }
    }
}
