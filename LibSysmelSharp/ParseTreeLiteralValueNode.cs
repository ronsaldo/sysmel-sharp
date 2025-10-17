namespace LibSysmelSharp
{
    public class ParseTreeLiteralValueNode : ParseTreeNode
    {
        public Value value;

        public ParseTreeLiteralValueNode(SourcePosition? sourcePosition, Value value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralValueNode()
        {
            return true;
        }

        public override Value AnalyzeAndEvaluateInEnvironment(Environment environment)
        {
            return value;
        }
    }
}
