using System.Diagnostics;

namespace LibSysmelSharp
{
    public class FunctionValue : Value
    {
        public Value? owner;
        public string? name;
        public DependentFunctionType signature;
        public Environment definitionEnvironment;
        public ParseTreeNode definitionBody;
        public SysmelType type;
        public bool isTemplate;
        public Dictionary<MemoizedFunctionKey, TemplateInstance> memoizedValues;
        public List<TemplateInstance> instances;
        public List<TemplateExtensionMethod> extensions;

        public HIRFunction? hirFunction;

        public FunctionValue(SourcePosition? sourcePosition, string? name, DependentFunctionType signature, Environment definitionEnvironment, ParseTreeNode definitionBody) : base(sourcePosition)
        {
            this.name = name;
            this.signature = signature;
            this.definitionEnvironment = definitionEnvironment;
            this.definitionBody = definitionBody;
            this.type = signature.TryToSimplifyFunctionType();
            this.isTemplate = signature.isForAll;
            this.memoizedValues = new Dictionary<MemoizedFunctionKey, TemplateInstance>();
            this.instances = new List<TemplateInstance>();
            this.extensions = new List<TemplateExtensionMethod>();
        }

        public override bool IsFunctionValue()
        {
            return true;
        }
        public override bool IsTemplate()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string? GetName()
        {
            return name;
        }

        public override void SetOwner(Value anOwner)
        {
            this.owner = anOwner;
        }

        public override string ToString()
        {
            if (name != null)
                return name;
            if (isTemplate)
                return "a Template";
            else
                return "a Function";
        }

        public override void EnsureAnalysis()
        {
            if (isTemplate)
                return;
            if (hirFunction != null)
                return;

            // Create he HIR Functon
            hirFunction = new HIRFunction(sourcePosition, name, type);

            FunctionAnalysisEnvironment functionAnalysisEnvironment = new FunctionAnalysisEnvironment(hirFunction, definitionEnvironment);

            foreach (ArgumentDefinitionValue argumentDef in signature.arguments)
            {
                string? argumentName = argumentDef.name;
                HIRArgument hirArgument = new HIRArgument(argumentDef.sourcePosition, argumentDef.type, argumentDef.name);
                hirFunction.AddArgument(hirArgument);

                if (argumentName != null)
                {
                    functionAnalysisEnvironment.SetSymbolBinding(argumentName, hirArgument);

                    if(argumentName == "self")
                    {
                        functionAnalysisEnvironment.receiverValue = hirArgument;
                        if(argumentDef.type.IsBehaviorType())
                        {
                            BehaviorType receiverBehavior = (BehaviorType)argumentDef.type;
                            if(receiverBehavior.superclass != null)
                            {
                                HIRSuperArgumentValue superArgument = new HIRSuperArgumentValue(argumentDef.sourcePosition, hirArgument, receiverBehavior.superclass, "super");
                                functionAnalysisEnvironment.SetSymbolBinding("super", superArgument);
                            }
                        }
                    }
                }
            }

            Debug.Assert(signature.resultTypeExpression.IsType());
            functionAnalysisEnvironment.returnTypeValue = (SysmelType)signature.resultTypeExpression;

            HIRBasicBlock allocaBlock = new HIRBasicBlock(sourcePosition, "alloca");
            hirFunction.AddBasicBlock(allocaBlock);

            HIRBasicBlock entryBlock = new HIRBasicBlock(sourcePosition, "entry");
            hirFunction.AddBasicBlock(entryBlock);

            HIRBuilder allocaBuilder = new HIRBuilder(functionAnalysisEnvironment, hirFunction);
            allocaBuilder.basicBlock = allocaBlock;

            HIRBuilder builder = new HIRBuilder(functionAnalysisEnvironment, hirFunction);
            builder.allocaBuilder = allocaBuilder;
            builder.basicBlock = entryBlock;

            HIRValue resultValue = definitionBody.AnalyzeAndBuildWithExpectedType(builder, functionAnalysisEnvironment.returnTypeValue);

            allocaBuilder.Branch(sourcePosition, entryBlock);
            if (!builder.IsLastTerminator())
            {
                if (functionAnalysisEnvironment.returnTypeValue.IsVoidType())
                    builder.ReturnVoid(sourcePosition);
                else
                    builder.ReturnValue(sourcePosition,resultValue);
            }
        }

        public override void DumpToConsole()
        {
            EnsureAnalysis();
            if (hirFunction != null)
                hirFunction.DumpToConsole();
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            EnsureAnalysis();

            MemoizedFunctionKey? memoizationKey = null;
            if (isTemplate)
            {
                memoizationKey = new MemoizedFunctionKey(callArguments);
                if (memoizedValues.ContainsKey(memoizationKey))
                    return memoizedValues[memoizationKey].instanceValue;
            }

            FunctionActivationEnvironment activationEnvironment = new FunctionActivationEnvironment(definitionEnvironment);
            foreach(ArgumentDefinitionValue argumentDef in signature.arguments)
            {
                if(argumentDef.name != null)
                {
                    Value argumentValue = callArguments[argumentDef.index];
                    activationEnvironment.SetSymbolBinding(argumentDef.name, argumentValue);
                }
            }

            Value resultType = signature.resultTypeExpression.AnalyzeAndEvaluateInEnvironment(activationEnvironment);
            if (!resultType.IsType())
                throw new SysmelException(signature.resultTypeExpression.sourcePosition, "Expected a result type expression.");

            SysmelType resultTypeValue = (SysmelType)resultType;

            Value resultValue = VoidValue.GetSingleton();
            LexicalEnvironment bodyEnvironment = new LexicalEnvironment(activationEnvironment);
            try
            {
                resultValue = definitionBody.AnalyzeAndEvaluateInEnvironmentWithExpectedType(bodyEnvironment, resultTypeValue);
            }
            catch (SysmelReturnException re)
            {
                resultValue = re.resultValue.AnalyzeAndEvaluateInEnvironmentWithExpectedType(bodyEnvironment, resultTypeValue);
            }

            if (isTemplate && memoizationKey != null)
            {
                TemplateInstance instance = new TemplateInstance(memoizationKey, resultValue);
                resultValue.SetOwner(this);
                memoizedValues[memoizationKey] = instance;
                instances.Add(instance);

                ApplyExtensionsToTemplateInstance(instance);
            }

            return resultValue;
        }

        public override Value AddExtensionMethodWithEnvironment(ParseTreeMethodNode methodNode, Environment environment)
        {
            TemplateExtensionMethod extensionMethod = new TemplateExtensionMethod(methodNode, environment);
            AddNewExtension(extensionMethod);
            return this;
        }

        void ApplyExtensionToTemplateInstance(TemplateInstance instanceValue, TemplateExtensionMethod extensionMethod)
        {
            FunctionActivationEnvironment activationEnvironment = new FunctionActivationEnvironment(extensionMethod.environment);
            foreach (ArgumentDefinitionValue argumentDef in signature.arguments)
            {
                if (argumentDef.name != null)
                {
                    Value argumentValue = instanceValue.instanceArguments.arguments[argumentDef.index];
                    activationEnvironment.SetSymbolBinding(argumentDef.name, argumentValue);
                }
            }

            LexicalEnvironment extensionBodyEnvironment = new LexicalEnvironment(activationEnvironment);
            if(instanceValue.instanceValue.IsType())
                extensionMethod.methodNode
                    .CopyWithSelfType((SysmelType)instanceValue.instanceValue)
                    .AnalyzeAndEvaluateInEnvironmentAddingToOwner(extensionBodyEnvironment, instanceValue.instanceValue);
            else
                extensionMethod.methodNode.AnalyzeAndEvaluateInEnvironmentAddingToOwner(extensionBodyEnvironment, instanceValue.instanceValue);
        }

        void ApplyExtensionsToTemplateInstance(TemplateInstance instanceValue)
        {
            foreach (TemplateExtensionMethod extension in extensions)
                ApplyExtensionToTemplateInstance(instanceValue, extension);
        }

        public void AddNewExtension(TemplateExtensionMethod extensionValue)
        {
            if (!isTemplate)
                throw new SysmelException(extensionValue.methodNode.sourcePosition, "Cannot add template extensions method to non-template " + ToString());
            extensions.Add(extensionValue);
            foreach (TemplateInstance instance in instances)
                ApplyExtensionToTemplateInstance(instance, extensionValue);
        }

    }
}
