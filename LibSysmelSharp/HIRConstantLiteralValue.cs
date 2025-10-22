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
        public override SysmelType AsSelfTypeWithDefaultType(SysmelType defaultResultType)
        {
            return literal.AsSelfTypeWithDefaultType(defaultResultType);
        }

        public override Value AsLiteralValue()
        {
            return literal;
        }

        public override string ToString()
        {
            return "constLiteral(" + literal.ToString() + ")"; 
        }
        public override bool IsDefaultCaseKey()
        {
            return literal.IsSymbolValue() && literal.AsSymbol() == "_";
        }

        public override Value GetValueInEvaluationContext(HIRFunctionActivationContext context)
        {
            return literal;
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            // Nothing is required here
        }
    }
}
