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

    }
}
