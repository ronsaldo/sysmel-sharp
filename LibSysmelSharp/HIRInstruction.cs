using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRInstruction : HIRFunctionLocalValue
    {
        public HIRBasicBlock? owner;

        public HIRInstruction? previousInstruction;
        public HIRInstruction? nextInstruction;

        public HIRInstruction(SourcePosition? sourcePosition, SysmelType type, string? name) : base(sourcePosition, type, name)
        {
        }

        public override bool IsInstruction()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  " + ToString());
        }

        public override string ToString()
        {
            return "%" + index.ToString();
        }
    }
}
