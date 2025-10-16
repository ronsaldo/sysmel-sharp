using System.Text;

namespace LibSysmelSharp
{
    public class TupleValue : Value
    {
        public Value[] elements;
        public SysmelType type;

        public TupleValue(SourcePosition? sourcePosition, Value[] elements, SysmelType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsTupleValue()
        {
            return true;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            for (int i = 0; i < elements.Length; ++i)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(elements[i]);
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}
