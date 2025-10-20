namespace LibSysmelSharp
{
    public class ParseTreeLiteralFloatNode : ParseTreeNode
    {
        public double value;

        public ParseTreeLiteralFloatNode(SourcePosition sourcePosition, double value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralFloatNode()
        {
            return true;
        }
        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return new FloatValue(sourcePosition, value, Intrinsics.GetSingleton().FloatType);
        }
        public override HIRValue AnalyzeAndBuild(HIRBuilder builder)
        {
            return builder.AddLiteral(sourcePosition, AnalyzeAndEvaluateInEnvironment(builder.environment));
        }

    }
}
