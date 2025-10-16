
namespace LibSysmelSharp
{
    public class IntegerValue : Value
    {
        public long value;
        public SysmelType type;
        
        public IntegerValue(SourcePosition? sourcePosition, long value, SysmelType type) : base(sourcePosition)
        {
            this.value = value;
            this.type = type;
        }

        public override SysmelType GetSysmelType()
        {
            return type;
        }

        public override long AsInteger()
        {
            return value;
        }
        public override double AsFloat()
        {
            return (double)value;
        }

        public override bool IsIntegerValue()
        {
            return true;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IntegerValue)
                return false;
            
            IntegerValue other = (IntegerValue)obj;
            return value == other.value && type.Equals(other.type);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode() ^ type.GetHashCode();
        }
    }
}
