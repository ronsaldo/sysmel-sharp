namespace LibSysmelSharp
{
    public class HIRArgument : HIRFunctionLocalValue
    {
        public HIRArgument(SourcePosition? sourcePosition, SysmelType type, string? name) : base(sourcePosition, type, name)
        {
        }

        public override bool IsArgumentValue()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            if(name == null)
                Console.WriteLine("%" + index + " := argument");
            else
                Console.WriteLine("%" + name + "." + index +" := argument");
        }
    }
}
