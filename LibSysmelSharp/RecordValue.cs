using System.Text;

namespace LibSysmelSharp
{
    public class RecordValue : Value
    {
        public Value[] elements;
        public RecordType type;

        public RecordValue(SourcePosition? sourcePosition, Value[] elements, RecordType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsRecordValue()
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
            builder.Append(type.ToString());
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
