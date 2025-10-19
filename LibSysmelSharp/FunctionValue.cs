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
        public Dictionary<MemoizedFunctionKey, Value> memoizedValues;
        public List<Value> instances;
        public List<Value> extensions;

        public FunctionValue(SourcePosition? sourcePosition, string? name, DependentFunctionType signature, Environment definitionEnvironment, ParseTreeNode definitionBody) : base(sourcePosition)
        {
            this.name = name;
            this.signature = signature;
            this.definitionEnvironment = definitionEnvironment;
            this.definitionBody = definitionBody;
            this.type = signature.TryToSimplifyFunctionType();
            this.isTemplate = signature.isForAll;
            this.memoizedValues = new Dictionary<MemoizedFunctionKey, Value>();
            this.instances = new List<Value>();
            this.extensions = new List<Value>();
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

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
            MemoizedFunctionKey? memoizationKey = null;
            if (isTemplate)
            {
                memoizationKey = new MemoizedFunctionKey(callArguments);
                if (memoizedValues.ContainsKey(memoizationKey))
                    return memoizedValues[memoizationKey];
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
                resultValue.SetOwner(this);
                memoizedValues[memoizationKey] = resultValue;
                instances.Add(resultValue);
            }

            return resultValue;
        }

    }
}
