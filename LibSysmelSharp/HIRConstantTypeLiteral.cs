namespace LibSysmelSharp
{
    internal class HIRConstantTypeLiteral: HIRConstant
    {
        public SysmelType typeLiteral;

        public HIRConstantTypeLiteral(SourcePosition? sourcePosition, SysmelType typeLiteral) : base(sourcePosition, typeLiteral.GetSysmelType())
        {
            this.typeLiteral = typeLiteral;
        }

        public override HIRValue AnalyzeAndBuildApplication(ParseTreeApplicationNode application, HIRBuilder builder)
        {
            return typeLiteral.AnalyzeAndBuildApplication(application, builder);
        }

        public override SysmelType AsSelfTypeWithDefaultType(SysmelType defaultResultType)
        {
            return typeLiteral.AsSelfTypeWithDefaultType(defaultResultType);
        }
        public override Value AsLiteralValue()
        {
            return typeLiteral;
        }

        public override bool IsConstantTypeLiteral()
        {
            return true;
        }

        public override Value GetValueInEvaluationContext(HIRFunctionActivationContext context)
        {
            return typeLiteral;
        }
        public override void EvaluateInActivationContext(HIRFunctionActivationContext context)
        {
            // Nothing is required here
        }

        public override HIRValue AnalyzeAndBuildMessageSendInEnvironment(ParseTreeMessageSendNode messageSend, string selector, HIRBuilder builder)
        {
            return typeLiteral.AnalyzeAndBuildMessageSendInEnvironment(messageSend, selector, builder);
        }
    }
}
