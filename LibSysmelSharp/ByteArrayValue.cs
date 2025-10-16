using System.Text;

namespace LibSysmelSharp
{
    public class ByteArrayValue : Value
    {
        public byte[] elements;
        public SysmelType type;

        public ByteArrayValue(SourcePosition? sourcePosition, byte[] elements, SysmelType type) : base(sourcePosition)
        {
            this.elements = elements;
            this.type = type;
        }

        public override bool IsByteArrayValue()
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
            builder.Append("#[");
            for (int i = 0; i < elements.Length; ++i)
            {
                if (i > 0)
                    builder.Append(". ");
                builder.Append(elements[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
