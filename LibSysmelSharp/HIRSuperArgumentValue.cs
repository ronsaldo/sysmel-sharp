using System;

namespace LibSysmelSharp
{
    public class HIRSuperArgumentValue : HIRFunctionLocalValue
    {
        public HIRArgument selfArgument;

        public HIRSuperArgumentValue(SourcePosition? sourcePosition, HIRArgument selfArgument, SysmelType type, string? name) : base(sourcePosition, type, name)
        {
            this.selfArgument = selfArgument;
        }

        public override bool IsSuperArgumentValue()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            if (name == null)
                Console.WriteLine("%" + index + " := super");
            else
                Console.WriteLine("%" + name + "." + index + " := super");
        }

    }
}
