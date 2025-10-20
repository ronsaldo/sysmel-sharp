namespace LibSysmelSharp
{
    public class HIRAssert : HIRInstruction
    {
        public HIRValue condition;

        public HIRAssert(SourcePosition? sourcePosition, HIRValue condition)
            : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.condition = condition;
        }

        public override bool IsAssertInstruction()
        {
            return true;
        }

        public override bool IsTerminator()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  assert: " + condition.ToString());
        }
    }
}
