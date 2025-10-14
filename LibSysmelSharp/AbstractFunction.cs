using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class AbstractFunction : Value
    {
        public SysmelType functionType;
        public string? name;
        public bool isMacro;

        public AbstractFunction(SourcePosition? sourcePosition, string? name, bool isMacro, SysmelType functionType) : base(sourcePosition)
        {
            this.functionType = functionType;
            this.name = name;
            this.isMacro = isMacro;
        }

        public override SysmelType GetSysmelType()
        {
            return functionType;
        }
    }
}
