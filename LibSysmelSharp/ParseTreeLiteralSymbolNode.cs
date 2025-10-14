namespace LibSysmelSharp
{
    public class ParseTreeLiteralSymbolNode : ParseTreeNode
    {
        public string value = "";

        public ParseTreeLiteralSymbolNode(SourcePosition sourcePosition, string value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralSymbolNode()
        {
            return true;
        }
    }
}
