using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class SysmelType : Value
    {
        public SysmelType(SourcePosition? sourcePosition) : base(sourcePosition)
        {
        }

        public virtual string? GetName()
        {
            return null;
        }
        
        public override bool IsType()
        {
            return true;
        }
    }
}
