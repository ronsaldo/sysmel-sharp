using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class ImmutableDictionaryValue : Value
    {
        public ImmutableDictionaryType type;
        public AssociationValue[] elements;

        public ImmutableDictionaryValue(SourcePosition? sourcePosition, ImmutableDictionaryType type, AssociationValue[] elements) : base(sourcePosition)
        {
            this.type = type;
            this.elements = elements;
        }

        public override bool IsImmutableDictionaryValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }
    }
}
