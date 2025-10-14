namespace LibSysmelSharp
{
    public class ParseTreeQuoteNode : ParseTreeNode
    {
        public ParseTreeNode expression;

        public ParseTreeQuoteNode(SourcePosition sourcePosition, ParseTreeNode expression) : base(sourcePosition)
        {
            this.expression = expression;
        }

        public override bool IsParseTreeQuoteNode()
        {
            return true;
        }
    }
}
