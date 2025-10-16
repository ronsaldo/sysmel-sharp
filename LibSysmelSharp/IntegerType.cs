namespace LibSysmelSharp
{
    public class IntegerType : NominalType
    {
        public int valueSize;
        public int valueAlignment;
        public bool isSigned;

        public IntegerType(string name, int valueSize, int valueAlignment, bool isSigned) : base(null, name)
        {
            this.valueSize = valueSize;
            this.valueAlignment = valueAlignment;
            this.isSigned = isSigned;
        }

        public override bool IsIntegerType()
        {
            return true;
        }
        public override Value MakeDefaultValue()
        {
            return new IntegerValue(null, 0, this);
        }
        public override Value MakePlusOneValue(Value sourceValue)
        {
            return new IntegerValue(null, sourceValue.AsInteger() + 1, this);
        }
    }
}
