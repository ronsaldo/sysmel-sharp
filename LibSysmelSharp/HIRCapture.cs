namespace LibSysmelSharp
{
    public class HIRCapture : HIRFunctionLocalValue
    {
        public HIRCapture(SourcePosition? sourcePosition, SysmelType type, string? name) : base(sourcePosition, type, name)
        {
        }

        public override bool IsCaptureValue()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            if (name == null)
                Console.WriteLine("%" + index + " := capture");
            else
                Console.WriteLine("%" + name + "." + index + " := capture");
        }
    }
}
