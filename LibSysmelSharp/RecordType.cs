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
        public override Value AnalyzeAndEvaluateApplicationInEnvironment(ParseTreeApplicationNode application, Environment environment)
        {
            int fieldCount = fields.Length;
            int argumentCount = application.arguments.Length;
            if (argumentCount > fields.Length)
                throw new SysmelException(application.sourcePosition, "Record expects at most " + fields.Length.ToString() + " arguments");

            Value[] recordElements = new Value[fields.Length];
            for(int i = 0; i < fieldCount; ++i)
            {
                FieldDefinition field = fields[i];

                if(i < argumentCount)
                    recordElements[i] = application.arguments[i].AnalyzeAndEvaluateInEnvironmentWithExpectedType(environment, field.type);
                else
                    recordElements[i] = field.type.MakeDefaultValue();
            }

            return new RecordValue(application.sourcePosition, recordElements, this);
        }
    }
}
