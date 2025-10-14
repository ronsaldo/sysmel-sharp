namespace LibSysmelSharp
{
    public class ParseTreeTupleNode : ParseTreeNode
    {
        public ParseTreeNode[] elements;
        public ParseTreeTupleNode(SourcePosition sourcePosition, ParseTreeNode[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }

        public override bool IsParseTreeTupleNode()
        {
            return true;
        }
    }
}
