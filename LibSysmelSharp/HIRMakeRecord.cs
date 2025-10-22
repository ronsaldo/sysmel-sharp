namespace LibSysmelSharp
{
    public class HIRMakeRecord : HIRInstruction
    {
        public HIRValue[] fields;

        public HIRMakeRecord(SourcePosition? sourcePosition, HIRValue[] fields, RecordType recordType) : base(sourcePosition, recordType, null)
        {
            this.fields = fields;
        }

        public override void DumpToConsole()
        {
            Console.Write("  " + ToString() + ":= makeRecord " + GetSysmelType().ToString() + " fields");
            foreach (HIRValue field in fields)
                Console.Write(", " + field.ToString());
            Console.WriteLine();
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            int fieldCount = fields.Length;
            Value[] evaluatedFields = new Value[fieldCount];
            for(int i = 0; i < fieldCount; ++i)
                evaluatedFields[i] = fields[i].GetValueInEvaluationContext(context);

            RecordValue record = new RecordValue(sourcePosition, evaluatedFields, (RecordType)type);
            context.SetCurrentInstructionValue(record);

        }
    }
}
