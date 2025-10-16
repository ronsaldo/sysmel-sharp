namespace LibSysmelSharp
{
    public class RecordType : SimpleType
    {
        public string name;
        public FieldDefinition[] fields;

        public RecordType(SourcePosition? sourcePosition, string name, FieldDefinition[] fields) : base(sourcePosition)
        {
            this.name = name;
            this.fields = fields;
        }
        public override bool IsRecordType()
        {
            return true;
        }
    }
}
