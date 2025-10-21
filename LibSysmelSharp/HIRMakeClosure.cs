namespace LibSysmelSharp
{
    public class HIRMakeClosure : HIRInstruction
    {
        public HIRFunction functionDefinition;
        public HIRValue[] captureValues;

        public HIRMakeClosure(SourcePosition? sourcePosition, HIRFunction functionDefinition, HIRValue[] captureValues) : base(sourcePosition, functionDefinition.GetSysmelType(), null)
        {
            this.functionDefinition = functionDefinition;
            this.captureValues = captureValues;
        }

        public override void DumpToConsole()
        {
            Console.Write("  " + ToString() + ":= makeClosure " + functionDefinition.ToString() + " captures");
            foreach (HIRValue capture in captureValues)
                Console.Write(", " + capture.ToString());
            Console.WriteLine();
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            int captureCount = captureValues.Length;
            Value functionValue = functionDefinition.GetValueInEvaluationContext(context);
            Value[] captureEvaluatedValues = new Value[captureCount];
            for (int i = 0; i < captureCount; ++i)
                captureEvaluatedValues[i] = captureValues[i].GetValueInEvaluationContext(context);
            ClosureValue closure = new ClosureValue(sourcePosition, null, false, false, GetSysmelType(), functionValue, captureEvaluatedValues);
            context.SetCurrentInstructionValue(closure);
        }
    }
}
