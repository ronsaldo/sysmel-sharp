namespace LibSysmelSharp
{
    public class HIRFunctionActivationContext
    {
        public SourcePosition? sourcePosition;
        public int instructionPC;
        public int pc;
        
        public List<HIRValue> intructions;
        public Value[] intructionValues;

        public Value? returnValue;


        public HIRFunctionActivationContext(SourcePosition? sourcePosition, List<HIRValue> instructions)
        {
            this.sourcePosition = sourcePosition;
            this.intructions = instructions;
            this.intructionValues = new Value[instructions.Count];

            for (int i = 0; i < instructions.Count; ++i)
                this.intructionValues[i] = VoidValue.GetSingleton();
        }

        public Value EvaluateInstructions()
        {
            int instructionCount = intructions.Count;
            while(pc < instructionCount)
            {
                // Fetch the instruction.
                instructionPC = pc++;
                HIRValue instruction = intructions[instructionPC];

                // Evaluate the intruction
                instruction.EvaluateInActivationContext(this);

                // Return expression.
                if (returnValue != null)
                    return returnValue;
            }

            throw new SysmelException(sourcePosition, "Reached beyond the end of a function instructions.");
        }

        public void SetCurrentInstructionValue(Value value)
        {
            intructionValues[instructionPC] = value;
        }
    }
}
