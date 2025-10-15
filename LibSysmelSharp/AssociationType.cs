
using System.Text;

namespace LibSysmelSharp
{
    public class AssociationType : SimpleType
    {
        public SysmelType keyType;
        public SysmelType valueType;

        public AssociationType(SourcePosition? sourcePosition, SysmelType keyType, SysmelType valueType) : base(sourcePosition)
        {
            this.keyType = keyType;
            this.valueType = valueType;
        }
        public override bool IsAssociationType()
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AssociationType)
                return false;

            AssociationType other = (AssociationType)obj;
            return keyType.Equals(other.keyType) && valueType.Equals(other.valueType);
        }

        public override int GetHashCode()
        {
            return keyType.GetHashCode() ^ valueType.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(");
            builder.Append(keyType.ToString());
            builder.Append(" : ");
            builder.Append(valueType.ToString());
            builder.Append(")");
            return builder.ToString();
        }
    }
}
