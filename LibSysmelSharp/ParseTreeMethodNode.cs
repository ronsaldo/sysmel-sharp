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

        public Value AnalyzeAndEvaluateInEnvironmentAddingToOwner(Environment environment, Value? owner)
        {
            DependentFunctionType functionTypeWithSignature = (DependentFunctionType)functionalSignature.AnalyzeAndEvaluateInEnvironment(environment);

            FunctionValue functionValue = new FunctionValue(sourcePosition, selector, false, false, functionTypeWithSignature, environment, body);

            if (owner != null)
                owner.WithSelectorAddMethod(selector, functionValue);

            functionValue.EnqueuePendingAnalysis();
            return functionValue;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return AnalyzeAndEvaluateInEnvironmentAddingToOwner(environment, null);
        }

        public ParseTreeMethodNode CopyWithSelfType(SysmelType newSelfType)
        {
            return new ParseTreeMethodNode(sourcePosition, functionalSignature.CopyWithSelfType(newSelfType), selector, body);
        }
    }
}
