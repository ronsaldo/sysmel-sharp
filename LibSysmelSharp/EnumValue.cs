using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysmelSharp
{
    public class EnumValue : Value
    {
        public EnumType type;
        public string name;
        public Value value;

        public EnumValue(SourcePosition? sourcePosition, EnumType type, string name, Value value) : base(sourcePosition)
        {
            this.type = type;
            this.name = name;
            this.value = value;
        }
        public override bool IsEnumValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string ToString()
        {
            return type.ToString() + " " + name;
        }

        public override bool AsBoolean()
        {
            return value.AsBoolean();
        }

        public override long AsInteger()
        {
            return value.AsInteger();
        }
        public override double AsFloat()
        {
            return value.AsFloat();
        }

        public override string AsString()
        {
            return value.AsString();
        }
        public override string AsSymbol()
        {
            return value.AsSymbol();
        }

    }
}
