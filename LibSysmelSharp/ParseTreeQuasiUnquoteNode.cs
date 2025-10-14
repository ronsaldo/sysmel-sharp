namespace LibSysmelSharp
{
    public class ParseTreeQuasiUnquoteNode : ParseTreeNode
    {
        public ParseTreeNode expression;

        public ParseTreeQuasiUnquoteNode(SourcePosition sourcePosition, ParseTreeNode expression) : base(sourcePosition)
        {
            this.expression = expression;
        }

        public override bool IsParseTreeQuasiUnquoteNode()
        {
            return true;
        }
    }
}
