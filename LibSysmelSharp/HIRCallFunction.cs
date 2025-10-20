using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRCallFunction : HIRInstruction
    {
        public HIRValue function;
        public HIRValue[] callArguments;

        public HIRCallFunction(SourcePosition? sourcePosition, HIRValue function, HIRValue[] callArguments, SysmelType resultType)
            : base(sourcePosition, resultType, null)
        {
            this.function = function;
            this.callArguments = callArguments;
        }

        public override bool IsCallFunctionInstruction()
        {
            return true;
        }
    }
}
