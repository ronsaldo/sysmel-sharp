
namespace LibSysmelSharp
{
    public class FloatValue : Value
    {
        public double value;
        public SysmelType type;

        public FloatValue(SourcePosition? sourcePosition, double value, SysmelType type) : base(sourcePosition)
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
            return (long)value;
        }

        public override double AsFloat()
        {
            return value;
        }

        public override bool IsFloatValue()
        {
            return true;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
