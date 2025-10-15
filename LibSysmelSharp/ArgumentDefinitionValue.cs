using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ArgumentDefinitionValue : Value
    {
        public string? name;
        public SysmelType type;
        public int index;
        public bool hasDependentUser = false;

        public ArgumentDefinitionValue(SourcePosition? sourcePosition, string? name, SysmelType type, int index) : base(sourcePosition)
        {
            this.name = name;
            this.type = type;
            this.index = index;
        }

        public override bool IsArgumentDefinitionValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }
    }
}
