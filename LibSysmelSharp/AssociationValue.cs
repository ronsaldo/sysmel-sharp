namespace LibSysmelSharp
{
    public class AssociationValue : Value
    {
        public AssociationType type;
        public Value key;
        public Value value;

        public AssociationValue(SourcePosition? sourcePosition, AssociationType type, Value key, Value value) : base(sourcePosition)
        {
            this.type = type;
            this.key = key;
            this.value = value;
        }

        public override bool IsAssociationValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }
    }
}
