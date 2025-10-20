namespace LibSysmelSharp
{
    public class HIRPhiSource : HIRInstruction
    {
        public HIRPhi destinationPhi;
        public HIRValue sourceValue;

        public HIRPhiSource(SourcePosition? sourcePosition, HIRPhi destinationPhi, HIRValue sourceValue) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.destinationPhi = destinationPhi;
            this.sourceValue = sourceValue;
        }

        public override bool IsPhiSource()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  phi " + destinationPhi.ToString() + " := " + sourceValue.ToString());
        }
    }
}
