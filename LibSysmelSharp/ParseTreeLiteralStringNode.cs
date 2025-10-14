namespace LibSysmelSharp
{
    public class ParseTreeLiteralStringNode : ParseTreeNode
    {
        public string value = "";
        public ParseTreeLiteralStringNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }
        public override bool IsParseTreeLiteralStringNode()
        {
            return true;
        }
    }
}
