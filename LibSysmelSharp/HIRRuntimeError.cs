using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRRuntimeError : HIRInstruction
    {
        public HIRValue errorMessage;

        public HIRRuntimeError(SourcePosition ?sourcePosition, HIRValue errorMessage)
            : base(sourcePosition, Intrinsics.GetSingleton().VoidType, null)
        {
            this.errorMessage = errorMessage;
        }

        public override bool IsRuntimeErrorInstruction()
        {
            return true;
        }

        public override bool IsTerminator()
        {
            return true;
        }

        public override void DumpToConsole()
        {
            Console.WriteLine("  runtimeError: " + errorMessage.ToString());
        }
    }
}
