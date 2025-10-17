namespace LibSysmelSharp
{
    public class ParseTreeMethodNode : ParseTreeNode
    {
        public ParseTreeFunctionalSignatureNode functionalSignature;
        public string selector;
        public ParseTreeNode body;

        public ParseTreeMethodNode(SourcePosition? sourcePosition, ParseTreeFunctionalSignatureNode functionalSignature, string selector, ParseTreeNode body) : base(sourcePosition)
        {
            this.functionalSignature = functionalSignature;
            this.selector = selector;
            this.body = body;
        }

        public override bool IsParseTreeMethodNode()
        {
            return true;
        }

        public Value AnalyzeAndEvaluateInEnvironmentAddignToOwner(Environment environment, Value? owner)
        {
            DependentFunctionType functionTypeWithSignature = (DependentFunctionType)functionalSignature.AnalyzeAndEvaluateInEnvironment(environment);

            FunctionValue functionValue = new FunctionValue(sourcePosition, selector, functionTypeWithSignature, environment, body);

            if (owner != null)
                owner.WithSelectorAddMethod(selector, functionValue);

            return functionValue;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return AnalyzeAndEvaluateInEnvironmentAddignToOwner(environment, null);
        }
    }
}
