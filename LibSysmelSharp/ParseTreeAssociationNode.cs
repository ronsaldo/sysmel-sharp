namespace LibSysmelSharp
{
    public class ParseTreeAssociationNode : ParseTreeNode
    {
        public ParseTreeNode key;
        public ParseTreeNode? value;
        public ParseTreeAssociationNode(SourcePosition sourcePosition, ParseTreeNode key, ParseTreeNode? value) : base(sourcePosition)
        {
            this.sourcePosition = sourcePosition;
            this.key = key;
            this.value = value;
        }

        public override bool IsParseTreeAssociationNode()
        {
            return true;
        }
    }
}
