using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class HIRFunctionLocalValue : HIRValue
    {
        public string? name;
        public int index;

        public HIRFunctionLocalValue(SourcePosition? sourcePosition, SysmelType type, string? name) : base(sourcePosition, type)
        {
            this.name = name;
        }
    }
}
