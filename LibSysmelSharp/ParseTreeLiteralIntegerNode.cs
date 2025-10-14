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
    }
}
