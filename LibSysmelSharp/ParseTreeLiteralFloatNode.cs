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
    }
}
