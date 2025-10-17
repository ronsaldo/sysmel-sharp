namespace LibSysmelSharp
{
    public class FunctionValue : Value
    {
        public string? name;
        public DependentFunctionType signature;
        public Environment definitionEnvironment;
        public ParseTreeNode definitionBody;
        public SysmelType type;

        public FunctionValue(SourcePosition? sourcePosition, string? name, DependentFunctionType signature, Environment definitionEnvironment, ParseTreeNode definitionBody) : base(sourcePosition)
        {
            this.name = name;
            this.signature = signature;
            this.definitionEnvironment = definitionEnvironment;
            this.definitionBody = definitionBody;
            this.type = signature.TryToSimplifyFunctionType();
        }

        public override bool IsFunctionValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override Value EvaluateWithArguments(Value[] callArguments)
        {
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

            return resultValue;
        }

    }
}
