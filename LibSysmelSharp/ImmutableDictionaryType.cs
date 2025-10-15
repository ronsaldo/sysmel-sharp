
using System.Text;

namespace LibSysmelSharp
{
    public class ImmutableDictionaryType : SimpleType
    {
        public AssociationType elementType;

        public ImmutableDictionaryType(SourcePosition? sourcePosition, AssociationType elementType) : base(sourcePosition)
        {
            this.elementType = elementType;
        }

        public override bool IsImmutableDictionaryType()
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ImmutableDictionaryType)
                return false;

            ImmutableDictionaryType other = (ImmutableDictionaryType)obj;
            return elementType.Equals(other.elementType);
        }

        public override int GetHashCode()
        {
            return elementType.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(elementType.ToString());
            builder.Append("#{}");
            return builder.ToString();
        }
    }
}
