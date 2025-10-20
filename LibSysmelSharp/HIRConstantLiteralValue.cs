namespace LibSysmelSharp
{
    public class HIRConstantLiteralValue : HIRConstant
    {
        public Value literal;

        public HIRConstantLiteralValue(SourcePosition? sourcePosition, Value literal) : base(sourcePosition, literal.GetSysmelType())
        {
            this.literal = literal;
        }

        public override bool IsConstantLiteralValue()
        {
            return true;
        }
    }
}
