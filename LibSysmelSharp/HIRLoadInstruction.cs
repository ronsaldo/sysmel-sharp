namespace LibSysmelSharp
{
    public class HIRLoadInstruction : HIRInstruction
    {
        public HIRValue storage;

        public HIRLoadInstruction(SourcePosition? sourcePosition, SysmelType type, HIRValue storage) : base(sourcePosition, type, null)
        {
            this.storage = storage;
        }

        public override bool IsLoadInstruction()
        {
            return true;
        }
        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + " := load " + storage.ToString());
        }
    }
}
