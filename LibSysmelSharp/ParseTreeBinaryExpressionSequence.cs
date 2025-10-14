namespace LibSysmelSharp
{
    public class ParseTreeBinaryExpressionSequence : ParseTreeNode
    {
        public ParseTreeNode[] operands;

        public ParseTreeBinaryExpressionSequence(SourcePosition sourcePosition, ParseTreeNode[] operands) : base(sourcePosition)
        {
            this.operands = operands;
        }

        public override bool IsParseTreeBinaryExpressionSequence()
        {
            return true;
        }

    }
}
