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

        public AbstractFunction(SourcePosition? sourcePosition, string? name, SysmelType functionType) : base(sourcePosition)
        {
            this.functionType = functionType;
            this.name = name;
        }

        public override SysmelType GetSysmelType()
        {
            return functionType;
        }
    }
}
