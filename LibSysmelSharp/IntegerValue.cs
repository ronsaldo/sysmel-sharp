
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

    }
}
