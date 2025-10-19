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
    }
}
