namespace LibSysmelSharp
{
    public class HIRReturnVoid : HIRInstruction
    {
        public HIRReturnVoid(SourcePosition? sourcePosition) : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
        }

        public override bool IsReturnVoid()
        {
            return true;
        }
        public override bool IsTerminator()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  returnVoid");
        }
    }
}
