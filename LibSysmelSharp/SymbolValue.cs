
namespace LibSysmelSharp
{
    public class SymbolValue : Value
    {
        public string value;
        public SysmelType type;

        public SymbolValue(SourcePosition? sourcePosition, string value, SysmelType type) : base(sourcePosition)
        {
            this.value = value;
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override string AsSymbol()
        {
            return value;
        }

        public override bool IsSymbolValue()
        {
            return true;
        }
        public override string ToString()
        {
            return "#\"" + value + "\"";
        }
        public override bool Equals(object? obj)
        {
            if (obj is not SymbolValue)
                return false;

            SymbolValue other = (SymbolValue)obj;
            return value == other.value && type.Equals(other.type);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ type.GetHashCode();
        }
    }
}
