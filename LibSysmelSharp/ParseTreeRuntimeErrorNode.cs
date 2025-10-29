namespace LibSysmelSharp
{
    public class ParseTreeRuntimeErrorNode : ParseTreeNode
    {
        public ParseTreeNode errorMessage;

        public ParseTreeRuntimeErrorNode(SourcePosition? sourcePosition, ParseTreeNode errorMessage) : base(sourcePosition)
        {
            this.errorMessage = errorMessage;
        }

        public override bool IsParseTreeRuntimeErrorNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            string errorMessageValue = errorMessage.AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, Intrinsics.GetSingleton().StringType).AsString();
            throw new SysmelException(sourcePosition, "Runtime error: " + errorMessageValue);
        }

        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            HIRValue errorMessageValue = errorMessage.AnalyzeAndBuildWithExpectedType(builder, Intrinsics.GetSingleton().StringType);
            builder.RuntimeError(sourcePosition, errorMessageValue);
            return builder.GetControlFlowEscapeLiteral(sourcePosition);
        }
    }
}
