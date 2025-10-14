namespace LibSysmelSharp
{
    public class Value
    {
        public SourcePosition? sourcePosition;

        public virtual bool IsParseTreeNode()
        {
            return false;
        }
    }
}
