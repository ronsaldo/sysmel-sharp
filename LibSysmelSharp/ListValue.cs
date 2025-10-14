using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ListValue : Value
    {
        public Value[] elements;
        public SysmelType type;

        public ListValue(SourcePosition? sourcePosition, Value[] elements, SysmelType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsListValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }
    }
}
