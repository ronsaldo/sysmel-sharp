namespace LibSysmelSharp
{
    public class TupleValue : Value
    {
        public Value[] elements;
        public SysmelType type;

        public TupleValue(SourcePosition? sourcePosition, Value[] elements, SysmelType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsTupleValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }
    }
}
