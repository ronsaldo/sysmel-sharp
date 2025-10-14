namespace LibSysmelSharp
{
    public class ParseTreeLiteralIntegerNode : ParseTreeNode
    {
        public long value;

        public override bool IsParseTreeLiteralIntegerNode()
        {
            return true;
        }
    }
}
