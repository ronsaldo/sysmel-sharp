namespace LibSysmelSharp
{
    public class Value
    {
        public SourcePosition? sourcePosition;

        public Value(SourcePosition sourcePosition)
        {
            this.sourcePosition = sourcePosition;
        }

        public virtual bool IsParseTreeNode()
        {
            return false;
        }
    }
}
