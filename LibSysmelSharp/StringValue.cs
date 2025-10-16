
namespace LibSysmelSharp
{
    public class StringValue : Value
    {
        public string value;
        public SysmelType type;

        public StringValue(SourcePosition? sourcePosition, string value, SysmelType type) : base(sourcePosition)
        {
            this.value = value;
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string AsString()
        {
            return value;
        }

        public override bool IsStringValue()
        {
            return true;
        }

        public override string ToString()
        {
            return "\"" + value + "\"";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not StringValue)
                return false;

            StringValue other = (StringValue)obj;
            return value == other.value && type.Equals(other.type);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ type.GetHashCode();
        }
    }
}
