using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class FieldDefinition : Value
    {
        public string name;
        public int index = 0;
        public SysmelType type;

        public FieldDefinition(SourcePosition? sourcePosition, string name, SysmelType type) : base(sourcePosition)
        {
            this.name = name;
            this.type = type;
        }
        
        public override bool IsFieldDefinition()
        {
            return true;
        }
    }
}
