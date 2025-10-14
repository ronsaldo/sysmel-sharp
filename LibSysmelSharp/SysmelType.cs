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

        public override Value? LookupSelector(string selector)
        {
            return null;
        }

        public virtual string? GetName()
        {
            return null;
        }
        
        public override bool IsType()
        {
            return true;
        }

        public virtual bool IsSatisfiedBy(SysmelType other)
        {
            return this == other;
        }
    }
}
