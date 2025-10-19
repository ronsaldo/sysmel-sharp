using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;

namespace LibSysmelSharp
{
    public class Intrinsics
    {
        static private Intrinsics? uniqueInstance;
        public Namespace globalNamespace;
        
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
        public IntegerType IntegerType;
        public NominalType FloatType;
        public NominalType StringType;
        public NominalType SymbolType;

        public NominalType FieldType;
        public NominalType FunctionType;
        public NominalType ParseTreeNodeType;
        public NominalType MacroContextType;
        public NominalType MetaBuilderType;
        public NominalType NamespaceType;

        public IntegerType Char8Type;
        public IntegerType Char16Type;
        public IntegerType Char32Type;

        public IntegerType UInt8Type;
        public IntegerType UInt16Type;
        public IntegerType UInt32Type;
        public IntegerType UInt64Type;

        public IntegerType Int8Type;
        public IntegerType Int16Type;
        public IntegerType Int32Type;
        public IntegerType Int64Type;

        public NominalType Float32Type;
        public NominalType Float64Type;

        public ListType DynamicListType; // TODO: Use an array type.
        public ListType ByteArrayType; // TODO: Use an array type.
        public AssociationType DynamicImmutableDictionaryAssociationType;
        public ImmutableDictionaryType DynamicImmutableDictionaryType;

        public ClassType ProtoObjectClassType;
        public ClassType ObjectClassType;
        public ClassType BehaviorClassType;
        public ClassType ClassClassType;
        public ClassType MetaclassClassType;

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

        NominalType CreateNominalType(string name)
        {
            NominalType type = new NominalType(null, name);
            globalNamespace.SetSymbolBinding(name, type);
            return type;
        }

        IntegerType CreateIntegerType(string name, int valueSize, int valueAlignment, bool isSigned)
        {
            IntegerType type = new IntegerType(name, valueSize, valueAlignment, isSigned);
            globalNamespace.SetSymbolBinding(name, type);
            return type;
        }

        ClassType CreateClassType(string name, BehaviorType? superclass, FieldDefinition[] metaFields, FieldDefinition[] fields)
        {
            BehaviorType? metaSuperclass = null;
            if (superclass != null)
                metaSuperclass = (BehaviorType)superclass.GetSysmelType();

            MetaclassType metaclass = new MetaclassType(null, metaSuperclass, metaFields);
            
            ClassType clazz = new ClassType(null, name, superclass, metaclass, fields);
            metaclass.thisClass = clazz;
            globalNamespace.SetSymbolBinding(name, clazz);
            return clazz;
        }

        void RegisterIntrinsicValue(string name, Value value)
        {
            globalNamespace.SetSymbolBinding(name, value);
        }

        internal Intrinsics()
        {
            globalNamespace = new Namespace(null, "__global__", null);
            TypeUniverse0 = new UniverseType(null, 0);
            globalNamespace.SetSymbolBinding("Type", TypeUniverse0);

            VoidType            = CreateNominalType("Void");
            UndefinedObjectType = CreateNominalType("UndefinedObject");
            DynamicType         = CreateNominalType("Dynamic");

            BooleanType   = CreateNominalType("Boolean");
            CharacterType = CreateNominalType("Character");
            IntegerType   = CreateIntegerType("Integer", 8, 8, true);
            FloatType     = CreateNominalType("Float");
            StringType    = CreateNominalType("String");
            SymbolType    = CreateNominalType("Symbol");

            Char8Type  = CreateIntegerType("Char8",  1, 1, false);
            Char16Type = CreateIntegerType("Char16", 2, 2, false);
            Char32Type = CreateIntegerType("Char32", 4, 4, false);

            UInt8Type  = CreateIntegerType("UInt8",  1, 1, false);
            UInt16Type = CreateIntegerType("UInt16", 2, 2, false);
            UInt32Type = CreateIntegerType("UInt32", 4, 4, false);
            UInt64Type = CreateIntegerType("UInt64", 8, 8, false);

            Int8Type  = CreateIntegerType("Int8",  1, 1, true);
            Int16Type = CreateIntegerType("Int16", 2, 2, true);
            Int32Type = CreateIntegerType("Int32", 4, 4, true);
            Int64Type = CreateIntegerType("Int64", 8, 8, true);

            Float32Type = CreateNominalType("Float32");
            Float64Type = CreateNominalType("Float64");

            FieldType         = CreateNominalType("FieldType");
            FunctionType      = CreateNominalType("Function");
            ParseTreeNodeType = CreateNominalType("ParseTreeNode");
            MacroContextType  = CreateNominalType("MacroContext");
            MetaBuilderType   = CreateNominalType("MetaBuilder");
            NamespaceType     = CreateNominalType("Namespace");

            DynamicListType = GetOrCreateListOfType(DynamicType);
            ByteArrayType = GetOrCreateListOfType(UInt8Type);
            DynamicImmutableDictionaryAssociationType = new AssociationType(null, SymbolType, DynamicType);
            DynamicImmutableDictionaryType = new ImmutableDictionaryType(null, DynamicImmutableDictionaryAssociationType);


            ProtoObjectClassType = CreateClassType("ProtoObject", null, [], []);
            ObjectClassType      = CreateClassType("Object", ProtoObjectClassType, [], []);
            BehaviorClassType    = CreateClassType("Behavior", ObjectClassType, [], []);
            ClassClassType       = CreateClassType("Class", BehaviorClassType, [], []);
            MetaclassClassType   = CreateClassType("Metaclass", BehaviorClassType, [], []);
            // Class short circuit
            ProtoObjectClassType.type.superclass = ClassClassType;

            RegisterIntrinsicValue("false", BooleanValue.GetFalse());
            RegisterIntrinsicValue("true",  BooleanValue.GetTrue());
            RegisterIntrinsicValue("nil",   NilValue.GetSingleton());
            RegisterIntrinsicValue("void",  VoidValue.GetSingleton());

            RegisterIntrinsicValue("let", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new LetMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("forall", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new ForAllMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("function", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new FunctionMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("namespace", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new NamespaceMetaBuilder(sourcePosition);
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
            globalNamespace.SetSymbolBinding(method.name, method);
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
            AddMethod(new PrimitiveFunction("switch:withCases:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode value = (ParseTreeNode)arguments[1];
                    ParseTreeNode cases = (ParseTreeNode)arguments[2];
                    return new ParseTreeSwitchNode(context.sourcePosition, value, cases);
                }));
            AddMethod(new PrimitiveFunction("while:do:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeWhileLoopNode(context.sourcePosition, condition, bodyExpression, null);
                }));
            AddMethod(new PrimitiveFunction("while:do:continueWith:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode continueExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeWhileLoopNode(context.sourcePosition, condition, bodyExpression, continueExpression);
                }));
            AddMethod(new PrimitiveFunction("do:while:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode condition = (ParseTreeNode)arguments[2];
                    return new ParseTreeDoWhileLoopNode(context.sourcePosition, bodyExpression, null, condition);
                }));
            AddMethod(new PrimitiveFunction("do:continueWith:while:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode continueExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode condition = (ParseTreeNode)arguments[3];
                    return new ParseTreeDoWhileLoopNode(context.sourcePosition, bodyExpression, continueExpression, condition);
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
            AddMethod(new PrimitiveFunction("Function:withArguments:resultType:definition:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode argumentsExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode resultTypeExpression = (ParseTreeNode)arguments[3];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[4];

                    ParseTreeArgumentDefinitionNode[] argumentDefinitions = argumentsExpression.ParseAsArgumentDefinitionsInEnvironment(context.environment);

                    ParseTreeFunctionalSignatureNode signature = new ParseTreeFunctionalSignatureNode(context.sourcePosition, argumentDefinitions, resultTypeExpression, null, false);
                    return new ParseTreeFunctionNode(context.sourcePosition, nameExpression, signature, bodyExpression);
                }));
            AddMethod(new PrimitiveFunction("Function:withArguments:definition:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode argumentsExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[3];

                    ParseTreeArgumentDefinitionNode[] argumentDefinitions = argumentsExpression.ParseAsArgumentDefinitionsInEnvironment(context.environment);

                    ParseTreeFunctionalSignatureNode signature = new ParseTreeFunctionalSignatureNode(context.sourcePosition, argumentDefinitions, null, null, false);
                    return new ParseTreeFunctionNode(context.sourcePosition, nameExpression, signature, bodyExpression);
                }));
            AddMethod(new PrimitiveFunction("Enum:withBaseType:values:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode baseType = (ParseTreeNode)arguments[2];
                    ParseTreeNode values = (ParseTreeNode)arguments[3];
                    return new ParseTreeEnumDefinitionNode(context.sourcePosition, nameExpression, baseType, values);
                }));
            AddMethod(new PrimitiveFunction("Record:withFields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeRecordDefinitionNode(context.sourcePosition, nameExpression, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("RecordWithFields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeRecordDefinitionNode(context.sourcePosition, null, fieldsExpression);
                }));

            AddMethod(new PrimitiveFunction("Class:withFields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, nameExpression, null, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("Class:withSuperclass:fields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode superclassExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, nameExpression, superclassExpression, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("ClassWithFields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, null, null, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("ClassWithSuperclass:fields:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode superclassExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, null, superclassExpression, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("namespace:definition:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeNamespaceNode(context.sourcePosition, nameExpression, bodyExpression);
                }));
            AddMethod(new PrimitiveFunction("ForAll:in:withDefinition:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode templateArguments = (ParseTreeNode)arguments[1];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[3];
                    
                    ParseTreeArgumentDefinitionNode[] argumentDefinitions = templateArguments.ParseAsArgumentDefinitionsInEnvironment(context.environment);
                    ParseTreeFunctionalSignatureNode signature = new ParseTreeFunctionalSignatureNode(context.sourcePosition, argumentDefinitions, null, null, true);
                    return new ParseTreeFunctionNode(context.sourcePosition, nameExpression, signature, bodyExpression);

                }));
            AddMethod(new PrimitiveFunction("public:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode exportListExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreePublicNode(context.sourcePosition, exportListExpression);
                }));

            AddMethod(new PrimitiveFunction("loadFileOnce:", true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fileNameExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeLoadFileOnce(context.sourcePosition, fileNameExpression);
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
        void RegisterFunctionPrimitives()
        {
            FunctionType.AddMethod(new PrimitiveFunction("dump", false,
                new SimpleFunctionType(null, [FunctionType], VoidType),
                (Value[] arguments) => {
                    Value functionValue = arguments[0];
                    functionValue.DumpToConsole();

                    return VoidValue.GetSingleton();
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
        void RegisterObjectTypePrimitives()
        {
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("initialize", false,
                new SimpleFunctionType(null, [ProtoObjectClassType], VoidType),
                (Value[] arguments) => {
                    return VoidValue.GetSingleton();
                }));
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("yourself", false,
                new SimpleFunctionType(null, [ProtoObjectClassType], ProtoObjectClassType),
                (Value[] arguments) => {
                    return arguments[0];
                }));
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("class", false,
                new SimpleFunctionType(null, [ProtoObjectClassType], BehaviorClassType),
                (Value[] arguments) => {
                    SysmelType type = arguments[0].GetSysmelType();
                    Debug.Assert(type.IsBehaviorType());
                    return type;
                }));

            // TODO: use a dependent type for the new result.
            BehaviorClassType.AddMethod(new PrimitiveFunction("basicNew", false,
                new SimpleFunctionType(null, [BehaviorClassType], ProtoObjectClassType),
                (Value[] arguments) => {
                    BehaviorType behavior = (BehaviorType)arguments[0];
                    return behavior.BasicNew();
                }));
            BehaviorClassType.AddMethod(new PrimitiveFunction("new", false,
                new SimpleFunctionType(null, [BehaviorClassType], ProtoObjectClassType),
                (Value[] arguments) => {
                    BehaviorType behavior = (BehaviorType)arguments[0];
                    return behavior.New();
                }));



        }

        void RegisterPrimitives()
        {
            RegisterControlFlowMacros();
            RegisterDefinitionMacros();
            RegisterFunctionPrimitives();
            RegisterStringPrimitives();
            RegisterTypePrimitives();
            RegisterIOPrimitive();
            RegisterIntegerTypePrimitives(IntegerType);
            RegisterObjectTypePrimitives();
        }
    }
}
