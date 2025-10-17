namespace LibSysmelSharp
{
    public class ParseTreeAddMethodNode : ParseTreeNode
    {
        public ParseTreeNode owner;
        public ParseTreeMethodNode method;

        public ParseTreeAddMethodNode(SourcePosition? sourcePosition, ParseTreeNode owner, ParseTreeMethodNode method) : base(sourcePosition)
        {
            this.owner = owner;
            this.method = method;
        }

        public override bool IsParseTreeAddMethodNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            Value ownerValue = owner.AnalyzeAndEvaluateInEnvironment(environment);
            if (!ownerValue.IsType())
                throw new SysmelException(sourcePosition, "Expected a type for adding a method.");

            ParseTreeMethodNode methodWithOwnerSelf = method.CopyWithSelfType((SysmelType)ownerValue);
            methodWithOwnerSelf.AnalyzeAndEvaluateInEnvironmentAddingToOwner(environment, (SysmelType)ownerValue);
            return ownerValue;
        }
    }
}
