namespace LibSysmelSharp
{
    public class HIRBranch : HIRInstruction
    {
        public HIRBasicBlock destination;

        public HIRBranch(SourcePosition? sourcePosition, HIRBasicBlock destination) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.destination = destination;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  branch " + destination.ToString());
        }

        public override bool IsBranchInstruction()
        {
            return true;
        }

        public override bool IsTerminator()
        {
            return true;
        }

        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            context.pc = destination.index;
        }
    }
}
