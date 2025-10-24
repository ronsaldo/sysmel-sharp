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

        public ListType DynamicListType;
        public ListType ByteArrayType;
        public AssociationType DynamicImmutableDictionaryAssociationType;
        public ImmutableDictionaryType DynamicImmutableDictionaryType;

        public ClassType ProtoObjectClassType;
        public ClassType ObjectClassType;
        public ClassType BehaviorClassType;
        public ClassType ClassClassType;
        public ClassType MetaclassClassType;

        public NominalType BasicBlockType;

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

            BasicBlockType = CreateNominalType("BasicBlockType");

            RegisterIntrinsicValue("false", BooleanValue.GetFalse());
            RegisterIntrinsicValue("true",  BooleanValue.GetTrue());
            RegisterIntrinsicValue("nil",   NilValue.GetSingleton());
            RegisterIntrinsicValue("void",  VoidValue.GetSingleton());

            RegisterIntrinsicValue("class", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new ClassMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("enum", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new EnumMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("forall", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new ForAllMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("function", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new FunctionMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("let", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new LetMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("namespace", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new NamespaceMetaBuilder(sourcePosition);
            }));
            RegisterIntrinsicValue("record", new MetaBuilderFactory(null, (SourcePosition? sourcePosition) => {
                return new RecordMetaBuilder(sourcePosition);
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

        void RegisterBooleanPrimitives()
        {
            BooleanType.AddMethod(new PrimitiveFunction("not", false, true,
                new SimpleFunctionType(null, [BooleanType], BooleanType),
                (Value[] arguments) => {
                    return BooleanValue.GetWithValue(!arguments[0].AsBoolean());
                }));
            BooleanType.AddMethod(new PrimitiveFunction("&&", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode firstOperand = (ParseTreeNode)arguments[1];
                    ParseTreeNode secondOperand = (ParseTreeNode)arguments[2];
                    ParseTreeLiteralValueNode booleanLiteral = new ParseTreeLiteralValueNode(context.sourcePosition, BooleanValue.GetWithValue(false));
                    return new ParseTreeIfSelectionNode(context.sourcePosition, firstOperand, secondOperand, booleanLiteral);
                }));
            BooleanType.AddMethod(new PrimitiveFunction("||", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode firstOperand = (ParseTreeNode)arguments[1];
                    ParseTreeNode secondOperand = (ParseTreeNode)arguments[2];
                    ParseTreeLiteralValueNode booleanLiteral = new ParseTreeLiteralValueNode(context.sourcePosition, BooleanValue.GetWithValue(true));
                    return new ParseTreeIfSelectionNode(context.sourcePosition, firstOperand, booleanLiteral, secondOperand);
                }));

        }

        void RegisterControlFlowMacros()
        {
            AddMethod(new PrimitiveFunction("if:then:else:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode trueExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode falseExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeIfSelectionNode(context.sourcePosition, condition, trueExpression, falseExpression);
                }));
            AddMethod(new PrimitiveFunction("if:then:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode trueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeIfSelectionNode(context.sourcePosition, condition, trueExpression, null);
                }));
            AddMethod(new PrimitiveFunction("switch:withCases:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode value = (ParseTreeNode)arguments[1];
                    ParseTreeNode cases = (ParseTreeNode)arguments[2];
                    return new ParseTreeSwitchNode(context.sourcePosition, value, cases);
                }));
            AddMethod(new PrimitiveFunction("while:do:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeWhileLoopNode(context.sourcePosition, condition, bodyExpression, null);
                }));
            AddMethod(new PrimitiveFunction("while:do:continueWith:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode condition = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode continueExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeWhileLoopNode(context.sourcePosition, condition, bodyExpression, continueExpression);
                }));
            AddMethod(new PrimitiveFunction("do:while:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode condition = (ParseTreeNode)arguments[2];
                    return new ParseTreeDoWhileLoopNode(context.sourcePosition, bodyExpression, null, condition);
                }));
            AddMethod(new PrimitiveFunction("do:continueWith:while:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode continueExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode condition = (ParseTreeNode)arguments[3];
                    return new ParseTreeDoWhileLoopNode(context.sourcePosition, bodyExpression, continueExpression, condition);
                }));


            AddMethod(new PrimitiveFunction("return:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode resultExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeReturnNode(context.sourcePosition, resultExpression);
                }));
            AddMethod(new PrimitiveFunction("assert:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode conditionExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeAssertNode(context.sourcePosition, conditionExpression);
                }));
            AddMethod(new PrimitiveFunction("error:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode errorMessage = (ParseTreeNode)arguments[1];
                    return new ParseTreeRuntimeErrorNode(context.sourcePosition, errorMessage);
                }));
        }

        void RegisterDefinitionMacros()
        {
            AddMethod(new PrimitiveFunction("let:with:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression= (ParseTreeNode)arguments[1];
                    ParseTreeNode initialValueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeVariableDefinitionNode(context.sourcePosition, nameExpression, null, initialValueExpression, false);
                }));
            AddMethod(new PrimitiveFunction("let:mutableWith:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode initialValueExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeVariableDefinitionNode(context.sourcePosition, nameExpression, null, initialValueExpression, true);
                }));
            AddMethod(new PrimitiveFunction("Function:withArguments:resultType:definition:", true, true,
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
            AddMethod(new PrimitiveFunction("Function:withArguments:definition:", true, true,
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
            AddMethod(new PrimitiveFunction("Enum:withBaseType:values:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode baseType = (ParseTreeNode)arguments[2];
                    ParseTreeNode values = (ParseTreeNode)arguments[3];
                    return new ParseTreeEnumDefinitionNode(context.sourcePosition, nameExpression, baseType, values);
                }));
            AddMethod(new PrimitiveFunction("Record:withFields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeRecordDefinitionNode(context.sourcePosition, nameExpression, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("RecordWithFields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeRecordDefinitionNode(context.sourcePosition, null, fieldsExpression);
                }));

            AddMethod(new PrimitiveFunction("Class:withFields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, nameExpression, null, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("Class:withSuperclass:fields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode superclassExpression = (ParseTreeNode)arguments[2];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[3];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, nameExpression, superclassExpression, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("ClassWithFields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, null, null, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("ClassWithSuperclass:fields:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode superclassExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode fieldsExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeClassDefinitionNode(context.sourcePosition, null, superclassExpression, null, fieldsExpression);
                }));
            AddMethod(new PrimitiveFunction("namespace:definition:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode nameExpression = (ParseTreeNode)arguments[1];
                    ParseTreeNode bodyExpression = (ParseTreeNode)arguments[2];
                    return new ParseTreeNamespaceNode(context.sourcePosition, nameExpression, bodyExpression);
                }));
            AddMethod(new PrimitiveFunction("ForAll:in:withDefinition:", true, true,
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
            AddMethod(new PrimitiveFunction("public:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode exportListExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreePublicNode(context.sourcePosition, exportListExpression);
                }));

            AddMethod(new PrimitiveFunction("loadFileOnce:", true, true,
                new SimpleFunctionType(null, [MacroContextType, ParseTreeNodeType], ParseTreeNodeType),
                (Value[] arguments) => {
                    MacroContextValue context = (MacroContextValue)arguments[0];
                    ParseTreeNode fileNameExpression = (ParseTreeNode)arguments[1];
                    return new ParseTreeLoadFileOnce(context.sourcePosition, fileNameExpression);
                }));
        }

        void RegisterIntegerTypePrimitives(NominalType integerType)
        {
            integerType.AddMethod(new PrimitiveFunction("negated", false, true,
                new SimpleFunctionType(null, [ integerType ], integerType),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, -arguments[0].AsInteger(), integerType);
                }));

            integerType.AddMethod(new PrimitiveFunction("+", false, true,
                new SimpleFunctionType(null, [ integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue + rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("-", false, true,
                new SimpleFunctionType(null, [ integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue - rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("*", false, true,
                new SimpleFunctionType(null, [integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue * rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("//", false, true,
                new SimpleFunctionType(null, [integerType, integerType ], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue / rightValue, integerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("%", false, true,
                new SimpleFunctionType(null, [integerType, integerType], integerType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return new IntegerValue(arguments[0].sourcePosition, leftValue % rightValue, integerType);
                }));

            integerType.AddMethod(new PrimitiveFunction("hash", false, true,
                new SimpleFunctionType(null, [integerType], IntegerType),
                (Value[] arguments) => {
                    long value = arguments[0].AsInteger();
                    return new IntegerValue(null, value.GetHashCode() & 0x7fffffff, IntegerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("=", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(arguments[0].Equals(arguments[1]));

                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue == rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("~=", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(!arguments[0].Equals(arguments[1]));

                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue != rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("<", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue < rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction("<=", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue <= rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction(">", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue > rightValue);
                }));
            integerType.AddMethod(new PrimitiveFunction(">=", false, true,
                new SimpleFunctionType(null, [integerType, integerType], BooleanType),
                (Value[] arguments) => {
                    long leftValue = arguments[0].AsInteger();
                    long rightValue = arguments[1].AsInteger();
                    return BooleanValue.GetWithValue(leftValue >= rightValue);
                }));

            integerType.AddMethod(new PrimitiveFunction("asCharacter", false, true,
                new SimpleFunctionType(null, [integerType], CharacterType),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, arguments[0].AsInteger(), CharacterType);
                }));
            integerType.AddMethod(new PrimitiveFunction("asInteger", false, true,
                new SimpleFunctionType(null, [integerType], IntegerType),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, arguments[0].AsInteger(), IntegerType);
                }));
            integerType.AddMethod(new PrimitiveFunction("asFloat", false, true,
                new SimpleFunctionType(null, [integerType], FloatType),
                (Value[] arguments) => {
                    return new FloatValue(arguments[0].sourcePosition, (double)arguments[0].AsInteger(), FloatType);
                }));


            integerType.AddMethod(new PrimitiveFunction("asChar8", false, true,
                new SimpleFunctionType(null, [integerType], Char8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char8Type.NormalizeInteger(arguments[0].AsInteger()), UInt8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asChar16", false, true,
                new SimpleFunctionType(null, [integerType], Char16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char16Type.NormalizeInteger(arguments[0].AsInteger()), Char16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asChar32", false, true,
                new SimpleFunctionType(null, [integerType], Char32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char32Type.NormalizeInteger(arguments[0].AsInteger()), Char32Type);
                }));
            
            integerType.AddMethod(new PrimitiveFunction("asUInt8", false, true,
                new SimpleFunctionType(null, [integerType], UInt8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt8Type.NormalizeInteger(arguments[0].AsInteger()), UInt8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asUInt16", false, true,
                new SimpleFunctionType(null, [integerType], UInt16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt16Type.NormalizeInteger(arguments[0].AsInteger()), UInt16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asUInt32", false, true,
                new SimpleFunctionType(null, [integerType], UInt32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt32Type.NormalizeInteger(arguments[0].AsInteger()), UInt32Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asUInt64", false, true,
                new SimpleFunctionType(null, [integerType], UInt64Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt64Type.NormalizeInteger(arguments[0].AsInteger()), UInt64Type);
                }));

            integerType.AddMethod(new PrimitiveFunction("asInt8", false, true,
                new SimpleFunctionType(null, [integerType], Int8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int8Type.NormalizeInteger(arguments[0].AsInteger()), Int8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asInt16", false, true,
                new SimpleFunctionType(null, [integerType], Int16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int16Type.NormalizeInteger(arguments[0].AsInteger()), Int16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asInt32", false, true,
                new SimpleFunctionType(null, [integerType], Int32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int32Type.NormalizeInteger(arguments[0].AsInteger()), Int32Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("asInt64", false, true,
                new SimpleFunctionType(null, [integerType], Int64Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int64Type.NormalizeInteger(arguments[0].AsInteger()), Int64Type);
                }));
        }

        void RegisterLiteralNumberPrimitives(NominalType integerType)
        {
            integerType.AddMethod(new PrimitiveFunction("c8", false, true,
                new SimpleFunctionType(null, [integerType], Char8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char8Type.NormalizeInteger(arguments[0].AsInteger()), UInt8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("c16", false, true,
                new SimpleFunctionType(null, [integerType], Char16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char16Type.NormalizeInteger(arguments[0].AsInteger()), Char16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("c32", false, true,
                new SimpleFunctionType(null, [integerType], Char32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Char32Type.NormalizeInteger(arguments[0].AsInteger()), Char32Type);
                }));

            integerType.AddMethod(new PrimitiveFunction("u8", false, true,
                new SimpleFunctionType(null, [integerType], UInt8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt8Type.NormalizeInteger(arguments[0].AsInteger()), UInt8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("u16", false, true,
                new SimpleFunctionType(null, [integerType], UInt16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt16Type.NormalizeInteger(arguments[0].AsInteger()), UInt16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("u32", false, true,
                new SimpleFunctionType(null, [integerType], UInt32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt32Type.NormalizeInteger(arguments[0].AsInteger()), UInt32Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("u64", false, true,
                new SimpleFunctionType(null, [integerType], UInt64Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, UInt64Type.NormalizeInteger(arguments[0].AsInteger()), UInt64Type);
                }));

            integerType.AddMethod(new PrimitiveFunction("i8", false, true,
                new SimpleFunctionType(null, [integerType], Int8Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int8Type.NormalizeInteger(arguments[0].AsInteger()), Int8Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("i16", false, true,
                new SimpleFunctionType(null, [integerType], Int16Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int16Type.NormalizeInteger(arguments[0].AsInteger()), Int16Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("i32", false, true,
                new SimpleFunctionType(null, [integerType], Int32Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int32Type.NormalizeInteger(arguments[0].AsInteger()), Int32Type);
                }));
            integerType.AddMethod(new PrimitiveFunction("i64", false, true,
                new SimpleFunctionType(null, [integerType], Int64Type),
                (Value[] arguments) => {
                    return new IntegerValue(arguments[0].sourcePosition, Int64Type.NormalizeInteger(arguments[0].AsInteger()), Int64Type);
                }));
        }

        void RegisterFunctionPrimitives()
        {
            FunctionType.AddMethod(new PrimitiveFunction("dump", false, false,
                new SimpleFunctionType(null, [FunctionType], VoidType),
                (Value[] arguments) => {
                    Value functionValue = arguments[0];
                    functionValue.DumpToConsole();

                    return VoidValue.GetSingleton();
                }));

        }

        void RegisterStringPrimitives()
        {
            StringType.AddMethod(new PrimitiveFunction("=", false, true,
                new SimpleFunctionType(null, [StringType, StringType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(arguments[0].Equals(arguments[1]));

                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return BooleanValue.GetWithValue(leftString == rightString);
                }));
            StringType.AddMethod(new PrimitiveFunction("~=", false, true,
                new SimpleFunctionType(null, [StringType, StringType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(!arguments[0].Equals(arguments[1]));

                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return BooleanValue.GetWithValue(leftString != rightString);
                }));
            StringType.AddMethod(new PrimitiveFunction("--", false, true,
                new SimpleFunctionType(null, [StringType, StringType], StringType),
                (Value[] arguments) => {
                    string leftString = arguments[0].AsString();
                    string rightString = arguments[1].AsString();
                    return new StringValue(null, leftString + rightString, StringType);
                }));
            StringType.AddMethod(new PrimitiveFunction("hash", false, true,
                new SimpleFunctionType(null, [StringType], IntegerType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    return new IntegerValue(null, str.GetHashCode() & 0x7fffffff, IntegerType);
                }));
            StringType.AddMethod(new PrimitiveFunction("size", false, true,
                new SimpleFunctionType(null, [StringType], IntegerType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    return new IntegerValue(null, str.Length, IntegerType);
                }));
            StringType.AddMethod(new PrimitiveFunction("at:", false, true,
                new SimpleFunctionType(null, [StringType, IntegerType], CharacterType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    long index = arguments[1].AsInteger();
                    return new IntegerValue(null, str[(int)index], CharacterType);
                }));
            StringType.AddMethod(new PrimitiveFunction("substringFrom:until:", false, true,
                new SimpleFunctionType(null, [StringType, IntegerType, IntegerType], StringType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    long startingIndex = arguments[1].AsInteger();
                    long endIndex = arguments[2].AsInteger();
                    string substring = str.Substring((int)startingIndex, (int)(endIndex - startingIndex));
                    return new StringValue(null, substring, StringType);
                }));
            StringType.AddMethod(new PrimitiveFunction("asSymbol", false, true,
                new SimpleFunctionType(null, [StringType], SymbolType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    return new SymbolValue(null, str, SymbolType);
                }));
            StringType.AddMethod(new PrimitiveFunction("parseAsFloat", false, true,
                new SimpleFunctionType(null, [StringType], FloatType),
                (Value[] arguments) => {
                    string str = arguments[0].AsString();
                    return new FloatValue(null, double.Parse(str), FloatType);
                }));
        }

        void RegisterSymbolPrimitives()
        {
            SymbolType.AddMethod(new PrimitiveFunction("=", false, true,
                new SimpleFunctionType(null, [SymbolType, SymbolType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(arguments[0].Equals(arguments[1]));

                    string leftString = arguments[0].AsSymbol();
                    string rightString = arguments[1].AsSymbol();
                    return BooleanValue.GetWithValue(leftString == rightString);
                }));
            SymbolType.AddMethod(new PrimitiveFunction("~=", false, true,
                new SimpleFunctionType(null, [SymbolType, SymbolType], BooleanType),
                (Value[] arguments) => {
                    if (arguments[0].IsNilValue() || arguments[1].IsNilValue())
                        return BooleanValue.GetWithValue(!arguments[0].Equals(arguments[1]));

                    string leftString = arguments[0].AsSymbol();
                    string rightString = arguments[1].AsSymbol();
                    return BooleanValue.GetWithValue(leftString != rightString);
                }));
            SymbolType.AddMethod(new PrimitiveFunction("hash", false, true,
                new SimpleFunctionType(null, [SymbolType], IntegerType),
                (Value[] arguments) => {
                    string str = arguments[0].AsSymbol();
                    return new IntegerValue(null, str.GetHashCode() & 0x7fffffff, IntegerType);
                }));
            SymbolType.AddMethod(new PrimitiveFunction("asString", false, true,
                new SimpleFunctionType(null, [SymbolType], StringType),
                (Value[] arguments) => {
                    string str = arguments[0].AsSymbol();
                    return new StringValue(null, str, StringType);
                }));
        }

        void RegisterTypePrimitives()
        {
            TypeUniverse0.AddMethod(new PrimitiveFunction("list", false, true,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).ListOf();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("pointer", false, true,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).Pointer();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("ref", false, true,
                new SimpleFunctionType(null, [TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).Ref();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("=>", false, true,
                new SimpleFunctionType(null, [TypeUniverse0, TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    SysmelType argumentType = (SysmelType)arguments[0];
                    SysmelType resultType = (SysmelType)arguments[1];
                    return argumentType.MakeSimpleFunctionWithResultType(resultType);
                }));

            TypeUniverse0.AddMethod(new PrimitiveFunction("basicNew", false, false,
                new SimpleFunctionType(null, [TypeUniverse0], DynamicType,
                (HIRValue[] arguments, SysmelType defaultResultType) => arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    SysmelType selfType = (SysmelType)arguments[0];
                    return selfType.BasicNew();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("basicNew:", false, false,
                new SimpleFunctionType(null, [TypeUniverse0, IntegerType], DynamicType,
                (HIRValue[] arguments, SysmelType defaultResultType) => arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    SysmelType selfType = (SysmelType)arguments[0];
                    long size = arguments[1].AsInteger();
                    return selfType.BasicNewWithSize(size);
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("new", false, false,
                new SimpleFunctionType(null, [TypeUniverse0], DynamicType,
                (HIRValue[] arguments, SysmelType defaultResultType) => arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    SysmelType selfType = (SysmelType)arguments[0];
                    return selfType.New();
                }));
            TypeUniverse0.AddMethod(new PrimitiveFunction("new:", false, false,
                new SimpleFunctionType(null, [TypeUniverse0, IntegerType], DynamicType,
                (HIRValue[] arguments, SysmelType defaultResultType) =>
                    arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    SysmelType selfType = (SysmelType)arguments[0];
                    long size = arguments[1].AsInteger();
                    return selfType.NewWithSize(size);
                }));
        }

        void RegisterIOPrimitive()
        {
            AddMethod(new PrimitiveFunction("write", false, false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.Write(arguments[0].AsString());
                    return VoidValue.GetSingleton();
                }));
            AddMethod(new PrimitiveFunction("writeLine", false, false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.WriteLine(arguments[0].AsString());
                    return VoidValue.GetSingleton();
                }));

            AddMethod(new PrimitiveFunction("print", false, false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.Write(arguments[0].ToString());
                    return VoidValue.GetSingleton();
                }));
            AddMethod(new PrimitiveFunction("printLine", false, false,
                new SimpleFunctionType(null, [DynamicType], VoidType),
                (Value[] arguments) => {
                    System.Console.WriteLine(arguments[0].ToString());
                    return VoidValue.GetSingleton();
                }));
        }
        void RegisterObjectTypePrimitives()
        {
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("initialize", false, false,
                new SimpleFunctionType(null, [ProtoObjectClassType], VoidType),
                (Value[] arguments) => {
                    return VoidValue.GetSingleton();
                }));
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("yourself", false, false,
                new SimpleFunctionType(null, [ProtoObjectClassType], ProtoObjectClassType),
                (Value[] arguments) => {
                    return arguments[0];
                }));
            ProtoObjectClassType.AddMethod(new PrimitiveFunction("class", false, false,
                new SimpleFunctionType(null, [ProtoObjectClassType], BehaviorClassType),
                (Value[] arguments) => {
                    SysmelType type = arguments[0].GetSysmelType();
                    Debug.Assert(type.IsBehaviorType());
                    return type;
                }));


            BehaviorClassType.AddMethod(new PrimitiveFunction("list", false, true,
                new SimpleFunctionType(null, [BehaviorClassType], TypeUniverse0),
                (Value[] arguments) => {
                    return ((SysmelType)arguments[0]).ListOf();
                }));
            BehaviorClassType.AddMethod(new PrimitiveFunction("=>", false, true,
                new SimpleFunctionType(null, [BehaviorClassType, TypeUniverse0], TypeUniverse0),
                (Value[] arguments) => {
                    SysmelType argumentType = (SysmelType)arguments[0];
                    SysmelType resultType = (SysmelType)arguments[1];
                    return argumentType.MakeSimpleFunctionWithResultType(resultType);
                }));
            BehaviorClassType.AddMethod(new PrimitiveFunction("basicNew", false, false,
                new SimpleFunctionType(null, [BehaviorClassType], ProtoObjectClassType,
                (HIRValue[] arguments, SysmelType defaultResultType) => arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    BehaviorType behavior = (BehaviorType)arguments[0];
                    return behavior.BasicNew();
                }));
            BehaviorClassType.AddMethod(new PrimitiveFunction("new", false, false,
                new SimpleFunctionType(null, [BehaviorClassType], ProtoObjectClassType,
                (HIRValue[] arguments, SysmelType defaultResultType) => arguments[0].AsSelfTypeWithDefaultType(defaultResultType)),
                (Value[] arguments) => {
                    BehaviorType behavior = (BehaviorType)arguments[0];
                    return behavior.New();
                }));
        }

        void RegisterPrimitives()
        {
            RegisterBooleanPrimitives();
            RegisterControlFlowMacros();
            RegisterDefinitionMacros();
            RegisterFunctionPrimitives();
            RegisterStringPrimitives();
            RegisterSymbolPrimitives();
            RegisterTypePrimitives();
            RegisterIOPrimitive();

            RegisterLiteralNumberPrimitives(IntegerType);
            RegisterLiteralNumberPrimitives(CharacterType);
            RegisterIntegerTypePrimitives(IntegerType);
            RegisterIntegerTypePrimitives(CharacterType);

            RegisterObjectTypePrimitives();
        }
    }
}
