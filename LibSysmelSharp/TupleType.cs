
using System.Text;

namespace LibSysmelSharp
{
    public class TupleType : SimpleType
    {
        public SysmelType[] elements;

        public TupleType(SourcePosition? sourcePosition, SysmelType[] elements) : base(sourcePosition)
        {
            this.elements = elements;
        }
        public override bool IsTupleType()
        {
            return true;
        }

        public override SimpleFunctionType MakeSimpleFunctionWithResultType(SysmelType resultType)
        {
            return new SimpleFunctionType(sourcePosition, elements, resultType);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not TupleType)
                return false;

            TupleType other = (TupleType)obj;
            return elements.SequenceEqual(other.elements);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (SysmelType element in elements)
                hashCode ^= element.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            for (int i = 0; i < elements.Length; ++i)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(elements[i].ToString());
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}
