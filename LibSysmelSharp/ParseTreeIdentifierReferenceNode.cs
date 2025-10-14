namespace LibSysmelSharp
{
    public class ParseTreeIdentifierReferenceNode : ParseTreeNode
    {
        public string value = "";

        public override bool IsParseTreeIdentifierReferenceNode()
        {
            return true;
        }
    }
}
