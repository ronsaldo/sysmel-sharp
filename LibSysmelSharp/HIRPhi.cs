namespace LibSysmelSharp
{
    public class HIRPhi : HIRInstruction
    {
        public HIRPhi(SourcePosition? sourcePosition, SysmelType type, string? name) : base(sourcePosition, type, name)
        {
        }

        public override bool IsPhi()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString() + ":= phi " + type.ToString());
        }
    }
}
