namespace LibSysmelSharp
{
    public class ParseTreeAddMethodNode : ParseTreeNode
    {
        public ParseTreeNode owner;
        public ParseTreeMethodNode method;

        public ParseTreeAddMethodNode(SourcePosition? sourcePosition, ParseTreeNode owner, ParseTreeMethodNode method) : base(sourcePosition)
        {
            this.owner = owner;
            this.method = method;
        }

        public override bool IsParseTreeAddMethodNode()
        {
            return true;
        }
    }
}
