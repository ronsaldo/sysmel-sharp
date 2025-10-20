namespace LibSysmelSharp
{
    public class ParseTreeLiteralStringNode : ParseTreeNode
    {
        public string value = "";
        public ParseTreeLiteralStringNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }
        public override bool IsParseTreeLiteralStringNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return new StringValue(sourcePosition, value, Intrinsics.GetSingleton().StringType);
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return builder.AddLiteral(sourcePosition, AnalyzeAndEvaluateInEnvironment(builder.environment));
        }
    }
}
