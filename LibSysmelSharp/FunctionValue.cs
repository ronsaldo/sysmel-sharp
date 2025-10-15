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
    }
}
