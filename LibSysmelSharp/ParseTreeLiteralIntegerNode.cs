namespace LibSysmelSharp
{
    public class ParseTreeLiteralIntegerNode : ParseTreeNode
    {
        public long value;
        public ParseTreeLiteralIntegerNode(SourcePosition sourcePosition, long value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralIntegerNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return new IntegerValue(sourcePosition, value, Intrinsics.GetSingleton().IntegerType);
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return builder.AddLiteral(sourcePosition, AnalyzeAndEvaluateInEnvironment(builder.environment));
        }
    }
}
