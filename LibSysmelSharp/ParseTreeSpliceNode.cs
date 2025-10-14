namespace LibSysmelSharp
{
    public class ParseTreeSpliceNode : ParseTreeNode
    {
        public ParseTreeNode expression;

        public ParseTreeSpliceNode(SourcePosition sourcePosition, ParseTreeNode expression) : base(sourcePosition)
        {
            this.expression = expression;
        }

        public override bool IsParseTreeSpliceNode()
        {
            return true;
        }
    }
}
