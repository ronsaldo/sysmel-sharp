
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
    }
}
