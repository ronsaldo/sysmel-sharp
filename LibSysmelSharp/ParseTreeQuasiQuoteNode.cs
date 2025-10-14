namespace LibSysmelSharp
{
    public class ParseTreeQuasiQuoteNode : ParseTreeNode
    {
        public ParseTreeNode expression;

        public ParseTreeQuasiQuoteNode(SourcePosition sourcePosition, ParseTreeNode expression) : base(sourcePosition)
        {
            this.expression = expression;
        }

        public override bool IsParseTreeQuasiQuoteNode()
        {
            return true;
        }
    }
}
