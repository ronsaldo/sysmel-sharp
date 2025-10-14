namespace LibSysmelSharp
{
    public class ParseTreeIdentifierReferenceNode : ParseTreeNode
    {
        public string value = "";
        public ParseTreeIdentifierReferenceNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeIdentifierReferenceNode()
        {
            return true;
        }
    }
}
