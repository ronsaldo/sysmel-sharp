namespace LibSysmelSharp
{
    public class ParseTreeLiteralCharacterNode : ParseTreeNode
    {
        public uint value;

        public ParseTreeLiteralCharacterNode(SourcePosition sourcePosition, uint value) : base(sourcePosition)
        {
            this.value = value;
        }

        public override bool IsParseTreeLiteralCharacterNode()
        {
            return true;
        }
    }
}
